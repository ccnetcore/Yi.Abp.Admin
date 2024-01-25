using System;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Yi.Framework.AuditLogging.Domain.Shared.Consts;

namespace Yi.Framework.AuditLogging.Domain.Entities;

[DisableAuditing]
[SugarTable("YiAuditLogAction")]
[SugarIndex($"index_{nameof(AuditLogId)}", nameof(AuditLogId), OrderByType.Asc)]
[SugarIndex($"index_{nameof(TenantId)}_{nameof(ExecutionTime)}", nameof(TenantId), OrderByType.Asc, nameof(ServiceName), OrderByType.Asc, nameof(MethodName), OrderByType.Asc, nameof(ExecutionTime), OrderByType.Asc)]
public class AuditLogActionEntity : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid AuditLogId { get; protected set; }

    public virtual string? ServiceName { get; protected set; }

    public virtual string? MethodName { get; protected set; }

    public virtual string? Parameters { get; protected set; }

    public virtual DateTime? ExecutionTime { get; protected set; }

    public virtual int? ExecutionDuration { get; protected set; }


    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }
    public AuditLogActionEntity()
    {
    }

    public AuditLogActionEntity(Guid id, Guid auditLogId, AuditLogActionInfo actionInfo, Guid? tenantId = null)
    {

        Id = id;
        TenantId = tenantId;
        AuditLogId = auditLogId;
        ExecutionTime = actionInfo.ExecutionTime;
        ExecutionDuration = actionInfo.ExecutionDuration;

        ServiceName = actionInfo.ServiceName.TruncateFromBeginning(AuditLogActionConsts.MaxServiceNameLength);
        MethodName = actionInfo.MethodName.TruncateFromBeginning(AuditLogActionConsts.MaxMethodNameLength);
        Parameters = actionInfo.Parameters.Length > AuditLogActionConsts.MaxParametersLength ? "" : actionInfo.Parameters;
    }
}
