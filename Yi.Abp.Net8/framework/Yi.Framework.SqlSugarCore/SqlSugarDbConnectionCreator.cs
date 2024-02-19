using System.Reflection;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore
{

    public class SqlSugarDbConnectionCreator: ISqlSugarDbConnectionCreator,ITransientDependency
    {
        public SqlSugarDbConnectionCreator(IOptions<DbConnOptions> options)
        {
            Options = options.Value;
        }
        public DbConnOptions Options { get; }

        public void SetDbAop(ISqlSugarClient currentDb)
        {
            currentDb.Aop.OnLogExecuting = this.OnLogExecuting;
            currentDb.Aop.OnLogExecuted = this.OnLogExecuted;
            currentDb.Aop.DataExecuting = this.DataExecuting;
            currentDb.Aop.DataExecuted = this.DataExecuted;
            OnSqlSugarClientConfig(currentDb);
        }



        public ConnectionConfig Build(Action<ConnectionConfig>? action=null)
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
                ConfigId= ConnectionStrings.DefaultConnectionStringName,
                DbType = dbConnOptions.DbType ?? DbType.Sqlite,
                ConnectionString = dbConnOptions.Url,
                IsAutoCloseConnection = true,
                SlaveConnectionConfigs = slavaConFig,
                //设置codefirst非空值判断
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (new NullabilityInfoContext()
                        .Create(c).WriteState is NullabilityState.Nullable)
                        {
                            p.IsNullable = true;
                        }

                        EntityService(c, p);
                    }
                },
                //这里多租户有个坑，无效的
                AopEvents = new AopEvents
                {
                    DataExecuted = DataExecuted,
                    DataExecuting = DataExecuting,
                    OnLogExecuted = OnLogExecuted,
                    OnLogExecuting = OnLogExecuting
                }

            };

            if (action is not null)
            {
                action.Invoke(connectionConfig);
            }
            #endregion
            return connectionConfig;
        }
        [DisablePropertyInjection]
        public Action<ISqlSugarClient> OnSqlSugarClientConfig { get; set; }

        [DisablePropertyInjection]
        public Action<object, DataAfterModel> DataExecuted { get; set; }

        [DisablePropertyInjection]
        public Action<object, DataFilterModel> DataExecuting { get; set; }

        [DisablePropertyInjection]
        public Action<string, SugarParameter[]> OnLogExecuting { get; set; }

        [DisablePropertyInjection]
        public Action<string, SugarParameter[]> OnLogExecuted { get; set; }

        [DisablePropertyInjection]
        public Action<PropertyInfo, EntityColumnInfo> EntityService { get; set; }
    }
}
