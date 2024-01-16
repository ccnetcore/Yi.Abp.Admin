using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using Mapster;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Yi.AuditLogging.SqlSugarCore.Entities;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.AuditLogging.SqlSugarCore;

public class SqlSugarCoreAuditLogRepository : SqlSugarObjectRepository<AuditLog, Guid>, IAuditLogRepository
{
    public virtual async Task<List<AuditLog>> GetListAsync(
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
        bool includeDetails = false,
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
            httpStatusCode,
            includeDetails
        );

        var auditLogs = await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(AuditLog.ExecutionTime) + " DESC" : sorting)
            .ToPageListAsync(skipCount, maxResultCount, cancellationToken);

        return auditLogs.Adapt<List<AuditLog>>();
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

        var totalCount = await query.CountAsync(cancellationToken);

        return totalCount;
    }

    protected virtual async Task<ISugarQueryable<AuditLogEntity>> GetListQueryAsync(
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
        return (await GetDbContextAsync()).Queryable<AuditLogEntity>()
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
        var result = await (await GetDbContextAsync()).Queryable<AuditLogEntity>()
            .Where(a => a.ExecutionTime < endDate.AddDays(1) && a.ExecutionTime > startDate)
            .OrderBy(t => t.ExecutionTime)
            .GroupBy(t => new { t.ExecutionTime.Date })
            .Select(g => new { Day = SqlFunc.AggregateMin(g.ExecutionTime), avgExecutionTime = SqlFunc.AggregateAvg(g.ExecutionDuration) })
            .ToListAsync(cancellationToken);

        return result.ToDictionary(element => element.Day.ClearTime(), element => (double)element.avgExecutionTime);
    }

    public virtual async Task<EntityChange> GetEntityChange(
        Guid entityChangeId,
        CancellationToken cancellationToken = default)
    {
        var entityChange = await (await GetDbContextAsync()).Queryable<EntityChange>()
                                .Where(x => x.Id == entityChangeId)
                                .OrderBy(x => x.Id)
                                .FirstAsync(cancellationToken);

        if (entityChange == null)
        {
            throw new EntityNotFoundException(typeof(EntityChange));
        }

        return entityChange;
    }

    public virtual async Task<List<EntityChange>> GetEntityChangeListAsync(
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

        return await query.OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(EntityChange.ChangeTime) + " DESC" : sorting)
            .ToPageListAsync(skipCount, maxResultCount, cancellationToken);
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

        var totalCount = await query.CountAsync(cancellationToken);

        return totalCount;
    }

    public virtual async Task<EntityChangeWithUsername> GetEntityChangeWithUsernameAsync(
        Guid entityChangeId,
        CancellationToken cancellationToken = default)
    {
        var auditLog = await (await GetDbContextAsync()).Queryable<AuditLogEntity>()
            .Where(x => x.EntityChanges.Any(y => y.Id == entityChangeId)).FirstAsync(cancellationToken);

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

        var query = dbContext.Queryable<EntityChange>()
                            .Where(x => x.EntityId == entityId && x.EntityTypeFullName == entityTypeFullName);
        return await query.LeftJoin<AuditLogEntity>((change, audit) => change.AuditLogId == audit.Id)
               .Select((change, audit) => new EntityChangeWithUsername { EntityChange = change, UserName = audit.UserName }, true)
                   .OrderByDescending(x => x.EntityChange.ChangeTime).ToListAsync(cancellationToken);

    }

    protected virtual async Task<ISugarQueryable<EntityChange>> GetEntityChangeListQueryAsync(
        Guid? auditLogId = null,
        DateTime? startTime = null,
        DateTime? endTime = null,
        EntityChangeType? changeType = null,
        string entityId = null,
        string entityTypeFullName = null,
        bool includeDetails = false)
    {
        return (await GetDbContextAsync())
            .Queryable<EntityChange>()
            .WhereIF(auditLogId.HasValue, e => e.AuditLogId == auditLogId)
            .WhereIF(startTime.HasValue, e => e.ChangeTime >= startTime)
            .WhereIF(endTime.HasValue, e => e.ChangeTime <= endTime)
            .WhereIF(changeType.HasValue, e => e.ChangeType == changeType)
            .WhereIF(!string.IsNullOrWhiteSpace(entityId), e => e.EntityId == entityId)
            .WhereIF(!string.IsNullOrWhiteSpace(entityTypeFullName), e => e.EntityTypeFullName.Contains(entityTypeFullName));
    }

    Task<List<AuditLog>> IAuditLogRepository.GetListAsync(string sorting, int maxResultCount, int skipCount, DateTime? startTime, DateTime? endTime, string httpMethod, string url, Guid? userId, string userName, string applicationName, string clientIpAddress, string correlationId, int? maxExecutionDuration, int? minExecutionDuration, bool? hasException, HttpStatusCode? httpStatusCode, bool includeDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }



    public Task<AuditLog> GetAsync(Expression<Func<AuditLog, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Expression<Func<AuditLog, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteDirectAsync(Expression<Func<AuditLog, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    IQueryable<AuditLog> IReadOnlyRepository<AuditLog>.WithDetails()
    {
        throw new NotImplementedException();
    }

    public IQueryable<AuditLog> WithDetails(params Expression<Func<AuditLog, object>>[] propertySelectors)
    {
        throw new NotImplementedException();
    }

    Task<IQueryable<AuditLog>> IReadOnlyRepository<AuditLog>.WithDetailsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IQueryable<AuditLog>> WithDetailsAsync(params Expression<Func<AuditLog, object>>[] propertySelectors)
    {
        throw new NotImplementedException();
    }

    Task<IQueryable<AuditLog>> IReadOnlyRepository<AuditLog>.GetQueryableAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<AuditLog>> GetListAsync(Expression<Func<AuditLog, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuditLog> InsertAsync(AuditLog entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task InsertManyAsync(IEnumerable<AuditLog> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuditLog> UpdateAsync(AuditLog entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateManyAsync(IEnumerable<AuditLog> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(AuditLog entity, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteManyAsync(IEnumerable<AuditLog> entities, bool autoSave = false, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    Task<AuditLog> IReadOnlyBasicRepository<AuditLog, Guid>.GetAsync(Guid id, bool includeDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<AuditLog?> IReadOnlyBasicRepository<AuditLog, Guid>.FindAsync(Guid id, bool includeDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<List<AuditLog>> IReadOnlyBasicRepository<AuditLog>.GetListAsync(bool includeDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    Task<List<AuditLog>> IReadOnlyBasicRepository<AuditLog>.GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
