using System.Collections;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Yi.Framework.SqlSugarCore.Abstractions;
using Check = Volo.Abp.Check;

namespace Yi.Framework.SqlSugarCore
{
    public class SqlSugarDbContext : ISqlSugarDbContext
    {
        /// <summary>
        /// SqlSugar 客户端
        /// </summary>
        public ISqlSugarClient SqlSugarClient { get; private set; }

        protected ICurrentUser CurrentUser => LazyServiceProvider.GetRequiredService<ICurrentUser>();
        private IAbpLazyServiceProvider LazyServiceProvider { get; }

        private IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();
        private ILoggerFactory Logger => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        private ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
        protected IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();
        protected virtual bool IsMultiTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant>() ?? false;

        protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ISoftDelete>() ?? false;

        private IEntityChangeEventHelper EntityChangeEventHelper => LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);
        public DbConnOptions Options => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

        private ISerializeService SerializeService=> LazyServiceProvider.LazyGetRequiredService<ISerializeService>();
        
        public void SetSqlSugarClient(ISqlSugarClient sqlSugarClient)
        {
            SqlSugarClient = sqlSugarClient;
        }
        public SqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;
            var connectionCreator = LazyServiceProvider.LazyGetRequiredService<ISqlSugarDbConnectionCreator>();
            connectionCreator.OnSqlSugarClientConfig = OnSqlSugarClientConfig;
            connectionCreator.EntityService = EntityService;
            connectionCreator.DataExecuting = DataExecuting;
            connectionCreator.DataExecuted = DataExecuted;
            connectionCreator.OnLogExecuting = OnLogExecuting;
            connectionCreator.OnLogExecuted = OnLogExecuted;
            SqlSugarClient = new SqlSugarClient(connectionCreator.Build(action: options =>
            {
                options.ConnectionString = GetCurrentConnectionString();
                options.DbType = GetCurrentDbType();
            }));
            //统一使用aop处理
            connectionCreator.SetDbAop(SqlSugarClient);
            //替换默认序列化器
            SqlSugarClient.CurrentConnectionConfig.ConfigureExternalServices.SerializeService = SerializeService;
        }

        /// <summary>
        /// db切换多库支持
        /// </summary>
        /// <returns></returns>
        protected virtual string GetCurrentConnectionString()
        {
            var connectionStringResolver = LazyServiceProvider.LazyGetRequiredService<IConnectionStringResolver>();
            var connectionString = connectionStringResolver.ResolveAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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



        /// <summary>
        /// 上下文对象扩展
        /// </summary>
        /// <param name="sqlSugarClient"></param>
        protected virtual void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
        {
            //需自定义扩展
            if (IsSoftDeleteFilterEnabled)
            {
                sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(u => u.IsDeleted == false);
            }
            if (IsMultiTenantFilterEnabled)
            {
                //表达式里只能有具体值，不能运算
                var expressionCurrentTenant = CurrentTenant.Id ?? null;
                sqlSugarClient.QueryFilter.AddTableFilter<IMultiTenant>(u => u.TenantId == expressionCurrentTenant);
            }
            CustomDataFilter(sqlSugarClient);
        }
        protected virtual void CustomDataFilter(ISqlSugarClient sqlSugarClient)
        {

        }
        protected virtual void DataExecuted(object oldValue, DataAfterModel entityInfo)
        {

        }

        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="entityInfo"></param>
        protected virtual void DataExecuting(object oldValue, DataFilterModel entityInfo)
        {
            //审计日志
            switch (entityInfo.OperationType)
            {
                case DataFilterType.UpdateByObject:

                    if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.LastModificationTime)))
                    {
                        if (!DateTime.MinValue.Equals(oldValue))
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                    if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.LastModifierId)))
                    {
                        if (CurrentUser.Id != null)
                        {
                            entityInfo.SetValue(CurrentUser.Id);
                        }
                    }
                    break;
                case DataFilterType.InsertByObject:
                    if (entityInfo.PropertyName.Equals(nameof(IEntity<Guid>.Id)))
                    {
                        //主键为空或者为默认最小值
                        if (Guid.Empty.Equals(oldValue))
                        {
                            entityInfo.SetValue(GuidGenerator.Create());
                        }
                    }

                    if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreationTime)))
                    {
                        //为空或者为默认最小值
                        if (DateTime.MinValue.Equals(oldValue))
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                    if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreatorId)))
                    {
                        if (CurrentUser.Id is not null)
                        {
                            entityInfo.SetValue(CurrentUser.Id);
                        }
                    }
                    
                    if (entityInfo.PropertyName.Equals(nameof(IMultiTenant.TenantId)))
                    {
                        if (CurrentTenant.Id is not null)
                        {
                            entityInfo.SetValue(CurrentTenant.Id);
                        }
                    }
                    break;
            }


            //领域事件
            switch (entityInfo.OperationType)
            {
                case DataFilterType.InsertByObject:
                    if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                    {
                        EntityChangeEventHelper.PublishEntityCreatedEvent(entityInfo.EntityValue);
                    }
                    break;
                case DataFilterType.UpdateByObject:
                    if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                    {
                        //软删除，发布的是删除事件
                        if (entityInfo.EntityValue is ISoftDelete softDelete)
                        {
                            if (softDelete.IsDeleted == true)
                            {
                                EntityChangeEventHelper.PublishEntityDeletedEvent(entityInfo.EntityValue);
                            }
                        }
                        else
                        {
                            EntityChangeEventHelper.PublishEntityUpdatedEvent(entityInfo.EntityValue);
                        }

                    }
                    break;
                case DataFilterType.DeleteByObject:
                    if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                    {
                        //这里sqlsugar有个特殊，删除会返回批量的结果
                        if (entityInfo.EntityValue is IEnumerable entityValues)
                        {
                            foreach (var entityValue in entityValues)
                            {

                                EntityChangeEventHelper.PublishEntityDeletedEvent(entityValue);
                            }
                        }
                    }
                    break;
            }

        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        protected virtual void OnLogExecuting(string sql, SugarParameter[] pars)
        {
            if (Options.EnabledSqlLog)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine("==========Yi-SQL执行:==========");
                sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
                sb.AppendLine("===============================");
                Logger.CreateLogger<SqlSugarDbContext>().LogDebug(sb.ToString());
            }

        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        protected virtual void OnLogExecuted(string sql, SugarParameter[] pars)
        {
            if (Options.EnabledSqlLog)
            {
                var sqllog = $"=========Yi-SQL耗时{SqlSugarClient.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
                Logger.CreateLogger<SqlSugarDbContext>().LogDebug(sqllog.ToString());
            }
        }

        /// <summary>
        /// 实体配置
        /// </summary>
        /// <param name="property"></param>
        /// <param name="column"></param>
        protected virtual void EntityService(PropertyInfo property, EntityColumnInfo column)
        {
            if (property.Name == nameof(IHasConcurrencyStamp.ConcurrencyStamp)) //带版本号并发更新
            {
                // column.IsOnlyIgnoreInsert = true;
                // column.IsOnlyIgnoreUpdate = true;
                column.IsEnableUpdateVersionValidation = true;
            }
            if (property.PropertyType == typeof(ExtraPropertyDictionary))
            {
                column.IsIgnore = true;
            }
            if (property.Name == nameof(Entity<object>.Id))
            {
                column.IsPrimarykey = true;
            }
        }

        public void BackupDataBase()
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
                    SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database, $"{Path.Combine(directoryName, fileName)}.sql");//mysql 只支持.net core
                    break;


                case DbType.Sqlite:
                    //Sqlite
                    SqlSugarClient.DbMaintenance.BackupDataBase(null, $"{fileName}.db"); //sqlite 只支持.net core
                    break;


                case DbType.SqlServer:
                    //SqlServer
                    SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database, $"{Path.Combine(directoryName, fileName)}.bak"/*服务器路径*/);//第一个参数库名 
                    break;


                default:
                    throw new NotImplementedException("其他数据库备份未实现");

            }





        }
    }
}
