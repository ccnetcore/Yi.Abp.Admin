using System.Net;
using Volo.Abp.Auditing;
using Yi.Framework.AuditLogging.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.AuditLogging.Domain.Repositories
{
    public interface IAuditLogRepository : ISqlSugarRepository<AuditLogAggregateRoot, Guid>
    {
        Task<Dictionary<DateTime, double>> GetAverageExecutionDurationPerDayAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<long> GetCountAsync(DateTime? startTime = null, DateTime? endTime = null, string httpMethod = null, string url = null, Guid? userId = null, string userName = null, string applicationName = null, string clientIpAddress = null, string correlationId = null, int? maxExecutionDuration = null, int? minExecutionDuration = null, bool? hasException = null, HttpStatusCode? httpStatusCode = null, CancellationToken cancellationToken = default);
        Task<EntityChangeEntity> GetEntityChange(Guid entityChangeId, CancellationToken cancellationToken = default);
        Task<long> GetEntityChangeCountAsync(Guid? auditLogId = null, DateTime? startTime = null, DateTime? endTime = null, EntityChangeType? changeType = null, string entityId = null, string entityTypeFullName = null, CancellationToken cancellationToken = default);
        Task<List<EntityChangeEntity>> GetEntityChangeListAsync(string sorting = null, int maxResultCount = 50, int skipCount = 0, Guid? auditLogId = null, DateTime? startTime = null, DateTime? endTime = null, EntityChangeType? changeType = null, string entityId = null, string entityTypeFullName = null, bool includeDetails = false, CancellationToken cancellationToken = default);
        Task<List<EntityChangeWithUsername>> GetEntityChangesWithUsernameAsync(string entityId, string entityTypeFullName, CancellationToken cancellationToken = default);
        Task<EntityChangeWithUsername> GetEntityChangeWithUsernameAsync(Guid entityChangeId);
        Task<List<AuditLogAggregateRoot>> GetListAsync(string sorting = null, int maxResultCount = 50, int skipCount = 0, DateTime? startTime = null, DateTime? endTime = null, string httpMethod = null, string url = null, Guid? userId = null, string userName = null, string applicationName = null, string clientIpAddress = null, string correlationId = null, int? maxExecutionDuration = null, int? minExecutionDuration = null, bool? hasException = null, HttpStatusCode? httpStatusCode = null, bool includeDetails = false);
    }
}
