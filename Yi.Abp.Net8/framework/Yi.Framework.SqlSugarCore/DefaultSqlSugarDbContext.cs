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

/// <summary>
/// 默认SqlSugar数据库上下文实现
/// </summary>
public class DefaultSqlSugarDbContext : SqlSugarDbContext
{
    #region Protected Properties

    /// <summary>
    /// 数据库连接配置选项
    /// </summary>
    protected DbConnOptions DbOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

    /// <summary>
    /// 当前用户服务
    /// </summary>
    protected ICurrentUser CurrentUserService => LazyServiceProvider.GetRequiredService<ICurrentUser>();

    /// <summary>
    /// GUID生成器
    /// </summary>
    protected IGuidGenerator GuidGeneratorService => LazyServiceProvider.LazyGetRequiredService<IGuidGenerator>();

    /// <summary>
    /// 日志工厂
    /// </summary>
    protected ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();

    /// <summary>
    /// 当前租户服务
    /// </summary>
    protected ICurrentTenant CurrentTenantService => LazyServiceProvider.LazyGetRequiredService<ICurrentTenant>();

    /// <summary>
    /// 数据过滤服务
    /// </summary>
    protected IDataFilter DataFilterService => LazyServiceProvider.LazyGetRequiredService<IDataFilter>();

    /// <summary>
    /// 工作单元管理器
    /// </summary>
    protected IUnitOfWorkManager UnitOfWorkManagerService => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();

    /// <summary>
    /// 实体变更事件帮助类
    /// </summary>
    protected IEntityChangeEventHelper EntityChangeEventHelperService =>
        LazyServiceProvider.LazyGetService<IEntityChangeEventHelper>(NullEntityChangeEventHelper.Instance);

    /// <summary>
    /// 是否启用多租户过滤
    /// </summary>
    protected virtual bool IsMultiTenantFilterEnabled => DataFilterService?.IsEnabled<IMultiTenant>() ?? false;

    /// <summary>
    /// 是否启用软删除过滤
    /// </summary>
    protected virtual bool IsSoftDeleteFilterEnabled => DataFilterService?.IsEnabled<ISoftDelete>() ?? false;

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public DefaultSqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider) 
        : base(lazyServiceProvider)
    {
    }

    /// <summary>
    /// 自定义数据过滤器
    /// </summary>
    protected override void CustomDataFilter(ISqlSugarClient sqlSugarClient)
    {
        // 配置软删除过滤器
        if (IsSoftDeleteFilterEnabled)
        {
            sqlSugarClient.QueryFilter.AddTableFilter<ISoftDelete>(entity => !entity.IsDeleted);
        }

        // 配置多租户过滤器
        if (IsMultiTenantFilterEnabled)
        {
            var currentTenantId = CurrentTenantService.Id;
            sqlSugarClient.QueryFilter.AddTableFilter<IMultiTenant>(entity => entity.TenantId == currentTenantId);
        }
    }

    /// <summary>
    /// 数据执行前的处理
    /// </summary>
    public override void DataExecuting(object oldValue, DataFilterModel entityInfo)
    {
        HandleAuditFields(oldValue, entityInfo);
        HandleEntityEvents(entityInfo);
        HandleDomainEvents(entityInfo);
    }

    #region Private Methods

    /// <summary>
    /// 处理审计字段
    /// </summary>
    private void HandleAuditFields(object oldValue, DataFilterModel entityInfo)
    {
        switch (entityInfo.OperationType)
        {
            case DataFilterType.UpdateByObject:
                HandleUpdateAuditFields(oldValue, entityInfo);
                break;
            case DataFilterType.InsertByObject:
                HandleInsertAuditFields(oldValue, entityInfo);
                break;
        }
    }

    /// <summary>
    /// 处理更新时的审计字段
    /// </summary>
    private void HandleUpdateAuditFields(object oldValue, DataFilterModel entityInfo)
    {
        if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.LastModificationTime)))
        {
            entityInfo.SetValue(DateTime.MinValue.Equals(oldValue) ? null : DateTime.Now);
        }
        else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.LastModifierId)) 
                 && entityInfo.EntityColumnInfo.PropertyInfo.PropertyType == typeof(Guid?))
        {
            entityInfo.SetValue(Guid.Empty.Equals(oldValue) ? null : CurrentUserService.Id);
        }
    }

    /// <summary>
    /// 处理插入时的审计字段
    /// </summary>
    private void HandleInsertAuditFields(object oldValue, DataFilterModel entityInfo)
    {
        if (entityInfo.PropertyName.Equals(nameof(IEntity<Guid>.Id)))
        {
            if (typeof(Guid) == entityInfo.EntityColumnInfo.PropertyInfo.PropertyType)
            {
                if (Guid.Empty.Equals(oldValue))
                {
                    entityInfo.SetValue(GuidGeneratorService.Create());
                }
            }
        }
        else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreationTime)))
        {
            if (DateTime.MinValue.Equals(oldValue))
            {
                entityInfo.SetValue(DateTime.Now);
            }
        }
        else if (entityInfo.PropertyName.Equals(nameof(IAuditedObject.CreatorId)))
        {
            if (typeof(Guid?) == entityInfo.EntityColumnInfo.PropertyInfo.PropertyType)
            {
                if (CurrentUserService.Id is not null)
                {
                    entityInfo.SetValue(CurrentUserService.Id);
                }
            }
        }
        else if (entityInfo.PropertyName.Equals(nameof(IMultiTenant.TenantId)))
        {
            if (CurrentTenantService.Id is not null)
            {
                entityInfo.SetValue(CurrentTenantService.Id);
            }
        }
    }

    /// <summary>
    /// 处理实体变更事件
    /// </summary>
    private void HandleEntityEvents(DataFilterModel entityInfo)
    {
        // 实体变更领域事件
        switch (entityInfo.OperationType)
        {
            case DataFilterType.InsertByObject:
                if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                {
                    EntityChangeEventHelperService.PublishEntityCreatedEvent(entityInfo.EntityValue);
                }
                break;
            case DataFilterType.UpdateByObject:
                if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
                {
                    if (entityInfo.EntityValue is ISoftDelete softDelete)
                    {
                        if (softDelete.IsDeleted == true)
                        {
                            EntityChangeEventHelperService.PublishEntityDeletedEvent(entityInfo.EntityValue);
                        }
                        else
                        {
                            EntityChangeEventHelperService.PublishEntityUpdatedEvent(entityInfo.EntityValue);
                        }
                    }
                    else
                    {
                        EntityChangeEventHelperService.PublishEntityUpdatedEvent(entityInfo.EntityValue);
                    }
                }
                break;
            case DataFilterType.DeleteByObject:
                if (entityInfo.EntityValue is IEnumerable entityValues)
                {
                    foreach (var entityValue in entityValues)
                    {
                        EntityChangeEventHelperService.PublishEntityDeletedEvent(entityValue);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 处理领域事件
    /// </summary>
    private void HandleDomainEvents(DataFilterModel entityInfo)
    {
        // 实体领域事件-所有操作类型
        if (entityInfo.PropertyName == nameof(IEntity<object>.Id))
        {
            var eventReport = CreateEventReport(entityInfo.EntityValue);
            PublishEntityEvents(eventReport);
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
            UnitOfWorkManagerService.Current?.AddOrReplaceLocalEvent(
                new UnitOfWorkEventRecord(localEvent.EventData.GetType(), localEvent.EventData, localEvent.EventOrder)
            );
        }

        foreach (var distributedEvent in changeReport.DistributedEvents)
        {
            UnitOfWorkManagerService.Current?.AddOrReplaceDistributedEvent(
                new UnitOfWorkEventRecord(distributedEvent.EventData.GetType(), distributedEvent.EventData, distributedEvent.EventOrder)
            );
        }
    }

    #endregion

    public override void OnLogExecuting(string sql, SugarParameter[] pars)
    {
        if (DbOptions.EnabledSqlLog)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("==========Yi-SQL执行:==========");
            sb.AppendLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
            sb.AppendLine("===============================");
            LoggerFactory.CreateLogger<DefaultSqlSugarDbContext>().LogDebug(sb.ToString());
        }
    }

    public override void OnLogExecuted(string sql, SugarParameter[] pars)
    {
        if (DbOptions.EnabledSqlLog)
        {
            var sqllog = $"=========Yi-SQL耗时{SqlSugarClient.Ado.SqlExecutionTime.TotalMilliseconds}毫秒=====";
            LoggerFactory.CreateLogger<SqlSugarDbContext>().LogDebug(sqllog.ToString());
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
}