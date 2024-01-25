using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Yi.Framework.AuditLogging.Domain.Entities;

namespace Yi.Framework.AuditLogging.Domain;

public interface IAuditLogInfoToAuditLogConverter
{
    Task<AuditLogAggregateRoot> ConvertAsync(AuditLogInfo auditLogInfo);
}
