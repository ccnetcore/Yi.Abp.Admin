using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Users;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore
{
    public class SqlSugarDbContext : ISqlSugarDbContext
    {
        /// <summary>
        /// SqlSugar 客户端
        /// </summary>
        public ISqlSugarClient SqlSugarClient { get; }
        public ICurrentUser CurrentUser => LazyServiceProvider.GetRequiredService<ICurrentUser>();

        private IAbpLazyServiceProvider LazyServiceProvider { get; }

        private IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();
        protected ILoggerFactory Logger => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        private ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
        public IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();
        protected virtual bool IsMultiTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant>() ?? false;

        protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ISoftDelete>() ?? false;

        public IEntityChangeEventHelper EntityChangeEventHelper => LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);
        protected DbConnOptions Options => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

        public SqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider)
        {
            LazyServiceProvider = lazyServiceProvider;
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
            SqlSugarClient = new SqlSugarClient(new ConnectionConfig()
            {
                //准备添加分表分库
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

                        EntityService(c,p);
                    }
                }
            },
         db =>
         {

             db.Aop.DataExecuting = DataExecuting;
             db.Aop.DataExecuted = DataExecuted;
             db.Aop.OnLogExecuting = OnLogExecuting;
             db.Aop.OnLogExecuted = OnLogExecuted;
             //扩展
             OnSqlSugarClientConfig(db);
         });
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
                sqlSugarClient.QueryFilter.AddTableFilter<IMultiTenant>(u => u.TenantId == GuidGenerator.Create());
            }
            CustomDataFilter();
        }
        protected virtual void CustomDataFilter()
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
                        if (CurrentUser != null)
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
                        if (oldValue is null || DateTime.MinValue.Equals(oldValue))
                        {
                            entityInfo.SetValue(DateTime.Now);
                        }
                    }
                    if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreatorId)))
                    {
                        if (CurrentUser != null)
                        {
                            entityInfo.SetValue(CurrentUser.Id);
                        }
                    }

                    //插入时，需要租户id,先预留
                    if (entityInfo.PropertyName.Equals(nameof(IMultiTenant.TenantId)))
                    {
                        if (CurrentTenant is not null)
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
                        EntityChangeEventHelper.PublishEntityUpdatedEvent(entityInfo.EntityValue);
                    }
                    break;
                case DataFilterType.DeleteByObject:
                    if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                    {
                        EntityChangeEventHelper.PublishEntityDeletedEvent(entityInfo.EntityValue);
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
                Logger.CreateLogger<SqlSugarDbContext>().LogDebug("Yi-SQL执行:" + UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
            }

        }

        /// <summary>
        /// 日志
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        protected virtual void OnLogExecuted(string sql, SugarParameter[] pars)
        {
        }

        /// <summary>
        /// 实体配置
        /// </summary>
        /// <param name="property"></param>
        /// <param name="column"></param>
        protected virtual void EntityService(PropertyInfo property, EntityColumnInfo column)
        { 
        
        }

        public void BackupDataBase()
        {
            string directoryName = "database_backup";
            string fileName = DateTime.Now.ToString($"yyyyMMdd_HHmmss")+ $"_{SqlSugarClient.Ado.Connection.Database}";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            switch (Options.DbType)
            {
                case DbType.MySql:
                    //MySql
                    SqlSugarClient.DbMaintenance.BackupDataBase(SqlSugarClient.Ado.Connection.Database, $"{Path.Combine(directoryName, fileName) }.sql");//mysql 只支持.net core
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
