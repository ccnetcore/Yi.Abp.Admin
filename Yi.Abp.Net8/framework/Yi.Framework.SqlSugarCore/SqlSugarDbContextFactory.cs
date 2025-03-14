using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Users;
using Yi.Framework.SqlSugarCore.Abstractions;
using Check = Volo.Abp.Check;

namespace Yi.Framework.SqlSugarCore
{
    /// <summary>
    /// SqlSugar数据库上下文工厂类
    /// 负责创建和配置SqlSugar客户端实例
    /// </summary>
    public class SqlSugarDbContextFactory : ISqlSugarDbContext
    {
        #region Properties

        /// <summary>
        /// SqlSugar客户端实例
        /// </summary>
        public ISqlSugarClient SqlSugarClient { get; private set; }

        /// <summary>
        /// 延迟服务提供者
        /// </summary>
        private IAbpLazyServiceProvider LazyServiceProvider { get; }

        /// <summary>
        /// 租户配置包装器
        /// </summary>
        private TenantConfigurationWrapper TenantConfigurationWrapper => 
            LazyServiceProvider.LazyGetRequiredService<TenantConfigurationWrapper>();

        /// <summary>
        /// 当前租户信息
        /// </summary>
        private ICurrentTenant CurrentTenant => 
            LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

        /// <summary>
        /// 数据库连接配置选项
        /// </summary>
        private DbConnOptions DbConnectionOptions => 
            LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

        /// <summary>
        /// 序列化服务
        /// </summary>
        private ISerializeService SerializeService => 
            LazyServiceProvider.LazyGetRequiredService<ISerializeService>();

        /// <summary>
        /// SqlSugar上下文依赖项集合
        /// </summary>
        private IEnumerable<ISqlSugarDbContextDependencies> SqlSugarDbContextDependencies =>
            LazyServiceProvider.LazyGetRequiredService<IEnumerable<ISqlSugarDbContextDependencies>>();

        /// <summary>
        /// 连接配置缓存字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConnectionConfig> ConnectionConfigCache = new();

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="lazyServiceProvider">延迟服务提供者</param>
        public SqlSugarDbContextFactory(IAbpLazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;

            // 异步获取租户配置
            var tenantConfiguration = AsyncHelper.RunSync(async () => await TenantConfigurationWrapper.GetAsync());
           
            // 构建数据库连接配置
            var connectionConfig = BuildConnectionConfig(options =>
            {
                options.ConnectionString = tenantConfiguration.GetCurrentConnectionString();
                options.DbType = GetCurrentDbType(tenantConfiguration.GetCurrentConnectionName());
            });

            // 创建SqlSugar客户端实例
            SqlSugarClient = new SqlSugarClient(connectionConfig);

            // 配置数据库AOP
            ConfigureDbAop(SqlSugarClient);
        }

        /// <summary>
        /// 配置数据库AOP操作
        /// </summary>
        /// <param name="sqlSugarClient">SqlSugar客户端实例</param>
        protected virtual void ConfigureDbAop(ISqlSugarClient sqlSugarClient)
        {
            // 配置序列化服务
            sqlSugarClient.CurrentConnectionConfig.ConfigureExternalServices.SerializeService = SerializeService;

            // 初始化AOP事件处理器
            Action<string, SugarParameter[]> onLogExecuting = null;
            Action<string, SugarParameter[]> onLogExecuted = null;
            Action<object, DataFilterModel> dataExecuting = null;
            Action<object, DataAfterModel> dataExecuted = null;
            Action<ISqlSugarClient> onClientConfig = null;

            // 按执行顺序聚合所有依赖项的AOP处理器
            foreach (var dependency in SqlSugarDbContextDependencies.OrderBy(x => x.ExecutionOrder))
            {
                onLogExecuting += dependency.OnLogExecuting;
                onLogExecuted += dependency.OnLogExecuted;
                dataExecuting += dependency.DataExecuting;
                dataExecuted += dependency.DataExecuted;
                onClientConfig += dependency.OnSqlSugarClientConfig;
            }

            // 配置SqlSugar客户端
            onClientConfig?.Invoke(sqlSugarClient);

            // 设置AOP事件
            sqlSugarClient.Aop.OnLogExecuting = onLogExecuting;
            sqlSugarClient.Aop.OnLogExecuted = onLogExecuted;
            sqlSugarClient.Aop.DataExecuting = dataExecuting;
            sqlSugarClient.Aop.DataExecuted = dataExecuted;
        }

        /// <summary>
        /// 构建数据库连接配置
        /// </summary>
        /// <param name="configAction">配置操作委托</param>
        /// <returns>连接配置对象</returns>
        protected virtual ConnectionConfig BuildConnectionConfig(Action<ConnectionConfig> configAction = null)
        {
            var dbConnOptions = DbConnectionOptions;

            // 验证数据库类型配置
            if (dbConnOptions.DbType is null)
            {
                throw new ArgumentException("未配置数据库类型(DbType)");
            }

            // 配置读写分离
            var slaveConfigs = new List<SlaveConnectionConfig>();
            if (dbConnOptions.EnabledReadWrite)
            {
                if (dbConnOptions.ReadUrl is null)
                {
                    throw new ArgumentException("启用读写分离但未配置读库连接字符串");
                }

                slaveConfigs.AddRange(dbConnOptions.ReadUrl.Select(url => 
                    new SlaveConnectionConfig { ConnectionString = url }));
            }

            // 创建连接配置
            var connectionConfig = new ConnectionConfig
            {
                ConfigId = ConnectionStrings.DefaultConnectionStringName,
                DbType = dbConnOptions.DbType ?? DbType.Sqlite,
                ConnectionString = dbConnOptions.Url,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = slaveConfigs,
                ConfigureExternalServices = CreateExternalServices(dbConnOptions)
            };

            // 应用额外配置
            configAction?.Invoke(connectionConfig);

            return connectionConfig;
        }

        /// <summary>
        /// 创建外部服务配置
        /// </summary>
        private ConfigureExternalServices CreateExternalServices(DbConnOptions dbConnOptions)
        {
            return new ConfigureExternalServices
            {
                EntityNameService = (type, entity) =>
                {
                    if (dbConnOptions.EnableUnderLine && !entity.DbTableName.Contains('_'))
                    {
                        entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName);
                    }
                },
                EntityService = (propertyInfo, columnInfo) =>
                {
                    // 配置空值处理
                    if (new NullabilityInfoContext().Create(propertyInfo).WriteState 
                        is NullabilityState.Nullable)
                    {
                        columnInfo.IsNullable = true;
                    }

                    // 处理下划线命名
                    if (dbConnOptions.EnableUnderLine && !columnInfo.IsIgnore 
                        && !columnInfo.DbColumnName.Contains('_'))
                    {
                        columnInfo.DbColumnName = UtilMethods.ToUnderLine(columnInfo.DbColumnName);
                    }

                    // 聚合所有依赖项的实体服务
                    Action<PropertyInfo, EntityColumnInfo> entityService = null;
                    foreach (var dependency in SqlSugarDbContextDependencies.OrderBy(x => x.ExecutionOrder))
                    {
                        entityService += dependency.EntityService;
                    }

                    entityService?.Invoke(propertyInfo, columnInfo);
                }
            };
        }

        /// <summary>
        /// 获取当前数据库类型
        /// </summary>
        /// <param name="tenantName">租户名称</param>
        /// <returns>数据库类型</returns>
        protected virtual DbType GetCurrentDbType(string tenantName)
        {
            return tenantName == ConnectionStrings.DefaultConnectionStringName 
                ? DbConnectionOptions.DbType!.Value 
                : GetDbTypeFromTenantName(tenantName) 
                  ?? throw new ArgumentException($"无法从租户名称{tenantName}中解析数据库类型");
        }

        /// <summary>
        /// 从租户名称解析数据库类型
        /// 格式：TenantName@DbType
        /// </summary>
        private DbType? GetDbTypeFromTenantName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            var atIndex = name.LastIndexOf('@');
            if (atIndex == -1 || atIndex == name.Length - 1)
            {
                return null;
            }

            var dbTypeString = name[(atIndex + 1)..];
            return Enum.TryParse<DbType>(dbTypeString, out var dbType) 
                ? dbType 
                : throw new ArgumentException($"不支持的数据库类型: {dbTypeString}");
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        public virtual void BackupDataBase()
        {
            const string backupDirectory = "database_backup";
            var fileName = $"{DateTime.Now:yyyyMMdd_HHmmss}_{SqlSugarClient.Ado.Connection.Database}";
            
            Directory.CreateDirectory(backupDirectory);

            switch (DbConnectionOptions.DbType)
            {
                case DbType.MySql:
                    SqlSugarClient.DbMaintenance.BackupDataBase(
                        SqlSugarClient.Ado.Connection.Database,
                        Path.Combine(backupDirectory, $"{fileName}.sql"));
                    break;

                case DbType.Sqlite:
                    SqlSugarClient.DbMaintenance.BackupDataBase(
                        null, 
                        $"{fileName}.db");
                    break;

                case DbType.SqlServer:
                    SqlSugarClient.DbMaintenance.BackupDataBase(
                        SqlSugarClient.Ado.Connection.Database,
                        Path.Combine(backupDirectory, $"{fileName}.bak"));
                    break;

                default:
                    throw new NotImplementedException($"数据库类型 {DbConnectionOptions.DbType} 的备份操作尚未实现");
            }
        }
    }
}