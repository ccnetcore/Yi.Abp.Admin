using System.Linq.Dynamic.Core;
using System.Net;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.AuditLogging.Domain;
using Yi.Framework.AuditLogging.Domain.Entities;
using Yi.Framework.AuditLogging.Domain.Repositories;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.Framework.AuditLogging.SqlSugarCore.Repositories;

public class SqlSugarCoreAuditLogRepository : SqlSugarRepository<AuditLogAggregateRoot, Guid>, IAuditLogRepository
{
    public SqlSugarCoreAuditLogRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
    {
    }

    /// <summary>
    /// 重写插入，支持导航属性
    /// </summary>
    /// <param name="insertObj"></param>
    /// <returns></returns>
    public override async Task<bool> InsertAsync(AuditLogAggregateRoot insertObj)
    {

        return await _Db.InsertNav<AuditLogAggregateRoot>(insertObj)
                 .Include(z1 => z1.Actions)
                 //.Include(z1 => z1.EntityChanges).ThenInclude(z2 => z2.PropertyChanges)
                 .ExecuteCommandAsync();
    }

    public virtual async Task<List<AuditLogAggregateRoot>> GetListAsync(
        string sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        DateTime? startTime = null,
        DateTime? endTime = null,
        string httpMethod = null,
        string url = null,
        Guid? userId = null,
        string userName = null,
        string applicationName = null,
        string clientIpAddress = null,
        string correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        HttpStatusCode? httpStatusCode = null,
        bool includeDetails = false)
    {
        var query = await GetListQueryAsync(
            startTime,
            endTime,
            httpMethod,
            url,
            userId,
            userName,
            applicationName,
            clientIpAddress,
            correlationId,
            maxExecutionDuration,
            minExecutionDuration,
            hasException,
            httpStatusCode,
            includeDetails
        );

        var auditLogs = await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? (nameof(AuditLogAggregateRoot.ExecutionTime) + " DESC") : sorting)
            .ToPageListAsync(skipCount, maxResultCount);

        return auditLogs;
    }

    public virtual async Task<long> GetCountAsync(
        DateTime? startTime = null,
        DateTime? endTime = null,
        string httpMethod = null,
        string url = null,
        Guid? userId = null,
        string userName = null,
        string applicationName = null,
        string clientIpAddress = null,
        string correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        HttpStatusCode? httpStatusCode = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetListQueryAsync(
            startTime,
            endTime,
            httpMethod,
            url,
            userId,
            userName,
            applicationName,
            clientIpAddress,
            correlationId,
            maxExecutionDuration,
            minExecutionDuration,
            hasException,
            httpStatusCode
        );

        var totalCount = await query.CountAsync();

        return totalCount;
    }

    protected virtual async Task<ISugarQueryable<AuditLogAggregateRoot>> GetListQueryAsync(
        DateTime? startTime = null,
        DateTime? endTime = null,
        string httpMethod = null,
        string url = null,
        Guid? userId = null,
        string userName = null,
        string applicationName = null,
        string clientIpAddress = null,
        string correlationId = null,
        int? maxExecutionDuration = null,
        int? minExecutionDuration = null,
        bool? hasException = null,
        HttpStatusCode? httpStatusCode = null,
        bool includeDetails = false)
    {
        var nHttpStatusCode = (int?)httpStatusCode;
        return _DbQueryable
            .WhereIF(startTime.HasValue, auditLog => auditLog.ExecutionTime >= startTime)
            .WhereIF(endTime.HasValue, auditLog => auditLog.ExecutionTime <= endTime)
            .WhereIF(hasException.HasValue && hasException.Value, auditLog => auditLog.Exceptions != null && auditLog.Exceptions != "")
            .WhereIF(hasException.HasValue && !hasException.Value, auditLog => auditLog.Exceptions == null || auditLog.Exceptions == "")
            .WhereIF(httpMethod != null, auditLog => auditLog.HttpMethod == httpMethod)
            .WhereIF(url != null, auditLog => auditLog.Url != null && auditLog.Url.Contains(url))
            .WhereIF(userId != null, auditLog => auditLog.UserId == userId)
            .WhereIF(userName != null, auditLog => auditLog.UserName == userName)
            .WhereIF(applicationName != null, auditLog => auditLog.ApplicationName == applicationName)
            .WhereIF(clientIpAddress != null, auditLog => auditLog.ClientIpAddress != null && auditLog.ClientIpAddress == clientIpAddress)
            .WhereIF(correlationId != null, auditLog => auditLog.CorrelationId == correlationId)
            .WhereIF(httpStatusCode != null && httpStatusCode > 0, auditLog => auditLog.HttpStatusCode == nHttpStatusCode)
            .WhereIF(maxExecutionDuration != null && maxExecutionDuration.Value > 0, auditLog => auditLog.ExecutionDuration <= maxExecutionDuration)
            .WhereIF(minExecutionDuration != null && minExecutionDuration.Value > 0, auditLog => auditLog.ExecutionDuration >= minExecutionDuration);
    }

    public virtual async Task<Dictionary<DateTime, double>> GetAverageExecutionDurationPerDayAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var result = await _DbQueryable
            .Where(a => a.ExecutionTime < endDate.AddDays(1) && a.ExecutionTime > startDate)
            .OrderBy(t => t.ExecutionTime)
            .GroupBy(t => new { t.ExecutionTime.Value.Date })
            .Select(g => new { Day = SqlFunc.AggregateMin(g.ExecutionTime), avgExecutionTime = SqlFunc.AggregateAvg(g.ExecutionDuration) })
            .ToListAsync();

        return result.ToDictionary(element => element.Day.Value.ClearTime(), element => (double)element.avgExecutionTime);
    }


    public virtual async Task<EntityChangeEntity> GetEntityChange(
        Guid entityChangeId,
        CancellationToken cancellationToken = default)
    {
        var entityChange = await (await GetDbContextAsync()).Queryable<EntityChangeEntity>()
                                .Where(x => x.Id == entityChangeId)
                                .OrderBy(x => x.Id)
                                .FirstAsync();

        if (entityChange == null)
        {
            throw new EntityNotFoundException(typeof(EntityChangeEntity));
        }

        return entityChange;
    }

    public virtual async Task<List<EntityChangeEntity>> GetEntityChangeListAsync(
        string sorting = null,
        int maxResultCount = 50,
        int skipCount = 0,
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string entityId = null,
        string entityTypeFullName = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetEntityChangeListQueryAsync(auditLogId, startTime, endTime, changeType, entityId, entityTypeFullName, includeDetails);

        return await query.OrderBy(sorting.IsNullOrWhiteSpace() ? (nameof(EntityChangeEntity.ChangeTime) + " DESC") : sorting)
            .ToPageListAsync(skipCount, maxResultCount);
    }

    public virtual async Task<long> GetEntityChangeCountAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string entityId = null,
        string entityTypeFullName = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetEntityChangeListQueryAsync(auditLogId, startTime, endTime, changeType, entityId, entityTypeFullName);

        var totalCount = await query.CountAsync();

        return totalCount;
    }

    public virtual async Task<EntityChangeWithUsername> GetEntityChangeWithUsernameAsync(
        Guid entityChangeId)
    {
        var auditLog = await _DbQueryable
            .Where(x => x.EntityChanges.Any(y => y.Id == entityChangeId)).FirstAsync();

        return new EntityChangeWithUsername()
        {
            EntityChange = auditLog.EntityChanges.First(x => x.Id == entityChangeId),
            UserName = auditLog.UserName
        };
    }

    public virtual async Task<List<EntityChangeWithUsername>> GetEntityChangesWithUsernameAsync(
        string entityId,
        string entityTypeFullName,
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();

        var query = dbContext.Queryable<EntityChangeEntity>()
                            .Where(x => x.EntityId == entityId && x.EntityTypeFullName == entityTypeFullName);
        return await query.LeftJoin<AuditLogAggregateRoot>((x, audit) => x.AuditLogId == audit.Id)
             .Select((x, audit) => new EntityChangeWithUsername { EntityChange = x, UserName = audit.UserName })
                .OrderByDescending(x => x.EntityChange.ChangeTime).ToListAsync();
    }

    protected virtual async Task<ISugarQueryable<EntityChangeEntity>> GetEntityChangeListQueryAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string entityId = null,
        string entityTypeFullName = null,
        bool includeDetails = false)
    {
        return (await GetDbContextAsync())
            .Queryable<EntityChangeEntity>()
            .WhereIF(auditLogId.HasValue, e => e.AuditLogId == auditLogId)
            .WhereIF(startTime.HasValue, e => e.ChangeTime >= startTime)
            .WhereIF(endTime.HasValue, e => e.ChangeTime <= endTime)
            .WhereIF(changeType.HasValue, e => e.ChangeType == changeType)
            .WhereIF(!string.IsNullOrWhiteSpace(entityId), e => e.EntityId == entityId)
            .WhereIF(!string.IsNullOrWhiteSpace(entityTypeFullName), e => e.EntityTypeFullName.Contains(entityTypeFullName));
    }
}
