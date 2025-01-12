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
    public class SqlSugarDbContextFactory : ISqlSugarDbContext
    {
        /// <summary>
        /// SqlSugar 客户端
        /// </summary>
        public ISqlSugarClient SqlSugarClient { get; private set; }

        private IAbpLazyServiceProvider LazyServiceProvider { get; }

        private ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
        public DbConnOptions Options => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

        private ISerializeService SerializeService => LazyServiceProvider.LazyGetRequiredService<ISerializeService>();

        private IEnumerable<ISqlSugarDbContextDependencies> SqlSugarDbContextDependencies =>
            LazyServiceProvider.LazyGetRequiredService<IEnumerable<ISqlSugarDbContextDependencies>>();

        private static readonly ConcurrentDictionary<string, ConnectionConfig> ConnectionConfigCache = new();

        public SqlSugarDbContextFactory(IAbpLazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;

            var connectionString = GetCurrentConnectionString();
            
            var connectionConfig =BuildConnectionConfig(action: options =>
            {
                options.ConnectionString = connectionString;
                options.DbType = GetCurrentDbType();
            });
            // var connectionConfig = ConnectionConfigCache.GetOrAdd(connectionString, (_) =>
            //     BuildConnectionConfig(action: options =>
            //     {
            //         options.ConnectionString = connectionString;
            //         options.DbType = GetCurrentDbType();
            //     }));
            SqlSugarClient = new SqlSugarClient(connectionConfig);
            //生命周期，以下都可以直接使用sqlsugardb了

            // Aop及多租户连接字符串和类型，需要单独设置
            // Aop操作不能进行缓存
            SetDbAop(SqlSugarClient);
        }

        /// <summary>
        /// 构建Aop-sqlsugaraop在多租户模式中，需单独设置
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        protected virtual void SetDbAop(ISqlSugarClient sqlSugarClient)
        {
            //替换默认序列化器
            sqlSugarClient.CurrentConnectionConfig.ConfigureExternalServices.SerializeService = SerializeService;

            //将所有，ISqlSugarDbContextDependencies进行累加
            Action<string, SugarParameter[]> onLogExecuting = null;
            Action<string, SugarParameter[]> onLogExecuted = null;
            Action<object, DataFilterModel> dataExecuting = null;
            Action<object, DataAfterModel> dataExecuted = null;
            Action<ISqlSugarClient> onSqlSugarClientConfig = null;

            foreach (var dependency in SqlSugarDbContextDependencies.OrderBy(x => x.ExecutionOrder))
            {
                onLogExecuting += dependency.OnLogExecuting;
                onLogExecuted += dependency.OnLogExecuted;
                dataExecuting += dependency.DataExecuting;
                dataExecuted += dependency.DataExecuted;

                onSqlSugarClientConfig += dependency.OnSqlSugarClientConfig;
            }

            //最先存放db操作
            onSqlSugarClientConfig(sqlSugarClient);

            sqlSugarClient.Aop.OnLogExecuting =onLogExecuting;
            sqlSugarClient.Aop.OnLogExecuted = onLogExecuted;

            sqlSugarClient.Aop.DataExecuting =dataExecuting;
            sqlSugarClient.Aop.DataExecuted =dataExecuted;
        }

        /// <summary>
        /// 构建连接配置
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected virtual ConnectionConfig BuildConnectionConfig(Action<ConnectionConfig>? action = null)
        {
            var dbConnOptions = Options;

            #region 组装options

            if (dbConnOptions.DbType is null)
            {
                throw new ArgumentException("DbType配置为空");
            }

            var slavaConFig = new List<SlaveConnectionConfig>();
            if (dbConnOptions.EnabledReadWrite)
            {
                if (dbConnOptions.ReadUrl is null)
                {
                    throw new ArgumentException("读写分离为空");
                }

                var readCon = dbConnOptions.ReadUrl;

                readCon.ForEach(s =>
                {
                    //如果是动态saas分库，这里的连接串都不能写死，需要动态添加，这里只配置共享库的连接
                    slavaConFig.Add(new SlaveConnectionConfig() { ConnectionString = s });
                });
            }

            #endregion

            #region 组装连接config

            var connectionConfig = new ConnectionConfig()
            {
                ConfigId = ConnectionStrings.DefaultConnectionStringName,
                DbType = dbConnOptions.DbType ?? DbType.Sqlite,
                ConnectionString = dbConnOptions.Url,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = slavaConFig,
                //设置codefirst非空值判断
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    // 处理表
                    EntityNameService = (type, entity) =>
                    {
                        if (dbConnOptions.EnableUnderLine && !entity.DbTableName.Contains('_'))
                            entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
                    },
                    EntityService = (c, p) =>
                    {
                        if (new NullabilityInfoContext()
                                .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }

                        if (dbConnOptions.EnableUnderLine && !p.IsIgnore && !p.DbColumnName.Contains('_'))
                            p.DbColumnName = UtilMethods.ToUnderLine(p.DbColumnName); // 驼峰转下划线

                        //将所有，ISqlSugarDbContextDependencies的EntityService进行累加
                        //额外的实体服务需要这里配置，

                        Action<PropertyInfo, EntityColumnInfo> entityService = null;
                        foreach (var dependency in SqlSugarDbContextDependencies.OrderBy(x => x.ExecutionOrder))
                        {
                            entityService += dependency.EntityService;
                        }

                        entityService(c, p);
                    }
                },
                //这里多租户有个坑，这里配置是无效的
                // AopEvents = new AopEvents
                // {
                //     DataExecuted = DataExecuted,
                //     DataExecuting = DataExecuting,
                //     OnLogExecuted = OnLogExecuted,
                //     OnLogExecuting = OnLogExecuting
                // }
            };

            if (action is not null)
            {
                action.Invoke(connectionConfig);
            }

            #endregion

            return connectionConfig;
        }

        /// <summary>
        /// db切换多库支持
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCurrentConnectionString()
        {
            var connectionStringResolver = LazyServiceProvider.LazyGetRequiredService<IConnectionStringResolver>();
            var connectionString =
                AsyncHelper.RunSync(() => connectionStringResolver.ResolveAsync());

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                Check.NotNull(Options.Url, "dbUrl未配置");
            }

            return connectionString!;
        }

        protected virtual DbType GetCurrentDbType()
        {
            if (CurrentTenant.Name is not null)
            {
                var dbTypeFromTenantName = GetDbTypeFromTenantName(CurrentTenant.Name);
                if (dbTypeFromTenantName is not null)
                {
                    return dbTypeFromTenantName.Value;
                }
            }

            Check.NotNull(Options.DbType, "默认DbType未配置！");
            return Options.DbType!.Value;
        }

        //根据租户name进行匹配db类型:  Test_Sqlite，[来自AI]
        private DbType? GetDbTypeFromTenantName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            // 查找下划线的位置
            int underscoreIndex = name.LastIndexOf('_');

            if (underscoreIndex == -1 || underscoreIndex == name.Length - 1)
            {
                return null;
            }

            // 提取 枚举 部分
            string enumString = name.Substring(underscoreIndex + 1);

            // 尝试将 尾缀 转换为枚举
            if (Enum.TryParse<DbType>(enumString, out DbType result))
            {
                return result;
            }

            // 条件不满足时返回 null
            return null;
        }

        public virtual void BackupDataBase()
        {
            string directoryName = "database_backup";
            string fileName = DateTime.Now.ToString($"yyyyMMdd_HHmmss") + $"_{SqlSugarClient.Ado.Connection.Database}";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            switch (Options.DbType)
            {
                case DbType.MySql:
                    //MySql
                    SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database,
                        $"{Path.Combine(directoryName, fileName)}.sql"); //mysql 只支持.net core
                    break;


                case DbType.Sqlite:
                    //Sqlite
                    SqlSugarClient.DbMaintenance.BackupDataBase(null, $"{fileName}.db"); //sqlite 只支持.net core
                    break;


                case DbType.SqlServer:
                    //SqlServer
                    SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database,
                        $"{Path.Combine(directoryName, fileName)}.bak" /*服务器路径*/); //第一个参数库名 
                    break;


                default:
                    throw new NotImplementedException("其他数据库备份未实现");
            }
        }
    }
}