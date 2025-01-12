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
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

public class DefaultSqlSugarDbContext : SqlSugarDbContext
{
    protected DbConnOptions Options => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;
    protected ICurrentUser CurrentUser => LazyServiceProvider.GetRequiredService<ICurrentUser>();
    protected IGuidGenerator GuidGenerator => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();
    protected ILoggerFactory Logger => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
    protected ICurrentTenant CurrentTenant => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();
    protected IDataFilter DataFilter => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();
    public IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
    protected virtual bool IsMultiTenantFilterEnabled => DataFilter?.IsEnabled<IMultiTenant>() ?? false;
    protected virtual bool IsSoftDeleteFilterEnabled => DataFilter?.IsEnabled<ISoftDelete>() ?? false;

    protected IEntityChangeEventHelper EntityChangeEventHelper =>
        LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);

    public DefaultSqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider) : base(lazyServiceProvider)
    {
    }

    protected override void CustomDataFilter(ISqlSugarClient sqlSugarClient)
    {
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
    }

    public override void DataExecuting(object oldValue, DataFilterModel entityInfo)
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
                else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.LastModifierId)))
                {
                    if (typeof(Guid?) == entityInfo.EntityColumnInfo.PropertyInfo.PropertyType)
                    {
                        if (CurrentUser.Id != null)
                        {
                            entityInfo.SetValue(CurrentUser.Id);
                        }
                    }
                }

                break;
            case DataFilterType.InsertByObject:

                if (entityInfo.PropertyName.Equals(nameof(IEntity<Guid>.Id)))
                {
                    //类型为guid
                    if (typeof(Guid) == entityInfo.EntityColumnInfo.PropertyInfo.PropertyType)
                    {
                        //主键为空或者为默认最小值
                        if (Guid.Empty.Equals(oldValue))
                        {
                            entityInfo.SetValue(GuidGenerator.Create());
                        }
                    }
                }

                else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreationTime)))
                {
                    //为空或者为默认最小值
                    if (DateTime.MinValue.Equals(oldValue))
                    {
                        entityInfo.SetValue(DateTime.Now);
                    }
                }
                else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreatorId)))
                {
                    //类型为guid
                    if (typeof(Guid?) == entityInfo.EntityColumnInfo.PropertyInfo.PropertyType)
                    {
                        if (CurrentUser.Id is not null)
                        {
                            entityInfo.SetValue(CurrentUser.Id);
                        }
                    }
                }

                else if (entityInfo.PropertyName.Equals(nameof(IMultiTenant.TenantId)))
                {
                    if (CurrentTenant.Id is not null)
                    {
                        entityInfo.SetValue(CurrentTenant.Id);
                    }
                }

                break;
        }


        //实体变更领域事件
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
        
      
        //实体领域事件-所有操作类型
        if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
        {
            var eventReport =  CreateEventReport(entityInfo.EntityValue);
            PublishEntityEvents(eventReport);
        }
    }

    public override void OnLogExecuting(string sql, SugarParameter[] pars)
    {
        if (Options.EnabledSqlLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("==========Yi-SQL执行:==========");
            sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
            sb.AppendLine("===============================");
            Logger.CreateLogger<DefaultSqlSugarDbContext>().LogDebug(sb.ToString());
        }
    }

    public override void OnLogExecuted(string sql, SugarParameter[] pars)
    {
        if (Options.EnabledSqlLog)
        {
            var sqllog = $"=========Yi-SQL耗时{SqlSugarClient.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
            Logger.CreateLogger<SqlSugarDbContext>().LogDebug(sqllog.ToString());
        }
    }

    public override void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo)
    {
        if (propertyInfo.Name == nameof(IHasConcurrencyStamp.ConcurrencyStamp)) //带版本号并发更新
        {
            entityColumnInfo.IsEnableUpdateVersionValidation = true;
        }

        if (propertyInfo.PropertyType == typeof(ExtraPropertyDictionary))
        {
            entityColumnInfo.IsIgnore = true;
        }

        if (propertyInfo.Name == nameof(Entity<object>.Id))
        {
            entityColumnInfo.IsPrimarykey = true;
        }
    }


    /// <summary>
    /// 创建领域事件报告
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected virtual EntityEventReport? CreateEventReport(object entity)
    {
        var eventReport = new EntityEventReport();
        
        //判断是否为领域事件-聚合根
        var generatesDomainEventsEntity = entity as IGeneratesDomainEvents;
        if (generatesDomainEventsEntity == null)
        {
            return eventReport;
        }

        var localEvents = generatesDomainEventsEntity.GetLocalEvents()?.ToArray();
        if (localEvents != null && localEvents.Any())
        {
            eventReport.DomainEvents.AddRange(
                localEvents.Select(
                    eventRecord => new DomainEventEntry(
                        entity,
                        eventRecord.EventData,
                        eventRecord.EventOrder
                    )
                )
            );
            generatesDomainEventsEntity.ClearLocalEvents();
        }

        var distributedEvents = generatesDomainEventsEntity.GetDistributedEvents()?.ToArray();
        if (distributedEvents != null && distributedEvents.Any())
        {
            eventReport.DistributedEvents.AddRange(
                distributedEvents.Select(
                    eventRecord => new DomainEventEntry(
                        entity,
                        eventRecord.EventData,
                        eventRecord.EventOrder)
                )
            );
            generatesDomainEventsEntity.ClearDistributedEvents();
        }
        
        return eventReport;
    }
    
    /// <summary>
    /// 发布领域事件
    /// </summary>
    /// <param name="changeReport"></param>
    private void PublishEntityEvents(EntityEventReport changeReport)
    {
        foreach (var localEvent in changeReport.DomainEvents)
        {
            UnitOfWorkManager.Current?.AddOrReplaceLocalEvent(
                new UnitOfWorkEventRecord(localEvent.EventData.GetType(), localEvent.EventData, localEvent.EventOrder)
            );
        }

        foreach (var distributedEvent in changeReport.DistributedEvents)
        {
            UnitOfWorkManager.Current?.AddOrReplaceDistributedEvent(
                new UnitOfWorkEventRecord(distributedEvent.EventData.GetType(), distributedEvent.EventData, distributedEvent.EventOrder)
            );
        }
    }
}