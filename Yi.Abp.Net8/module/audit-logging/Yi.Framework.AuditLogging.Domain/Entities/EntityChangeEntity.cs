using System.Reflection;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Yi.Framework.AuditLogging.Domain.Shared.Consts;

namespace Yi.Framework.AuditLogging.Domain.Entities
{
    [SugarTable("YiEntityChange")]
    [SugarIndex($"index_{nameof(AuditLogId)}", nameof(AuditLogId), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(TenantId)}_{nameof(EntityId)}", nameof(TenantId), OrderByType.Asc, nameof(EntityTypeFullName), OrderByType.Asc, nameof(EntityId), OrderByType.Asc)]
    public class EntityChangeEntity : Entity<Guid>, IMultiTenant
    {
        public EntityChangeEntity() { }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public virtual Guid AuditLogId { get; protected set; }

        public virtual Guid? TenantId { get; protected set; }

        public virtual DateTime? ChangeTime { get; protected set; }

        public virtual EntityChangeType? ChangeType { get; protected set; }

        public virtual Guid? EntityTenantId { get; protected set; }

        public virtual string? EntityId { get; protected set; }

        public virtual string? EntityTypeFullName { get; protected set; }
        [Navigate(NavigateType.OneToMany, nameof(EntityPropertyChangeEntity.EntityChangeId))]
        public virtual List<EntityPropertyChangeEntity> PropertyChanges { get; protected set; }


        public EntityChangeEntity(
       IGuidGenerator guidGenerator,
       Guid auditLogId,
       EntityChangeInfo entityChangeInfo,
       Guid? tenantId = null)
        {
            Id = guidGenerator.Create();
            AuditLogId = auditLogId;
            TenantId = tenantId;
            ChangeTime = entityChangeInfo.ChangeTime;
            ChangeType = entityChangeInfo.ChangeType;
            EntityTenantId = entityChangeInfo.EntityTenantId;
            EntityId = entityChangeInfo.EntityId.Truncate(EntityChangeConsts.MaxEntityTypeFullNameLength);
            EntityTypeFullName = entityChangeInfo.EntityTypeFullName.TruncateFromBeginning(EntityChangeConsts.MaxEntityTypeFullNameLength);

            PropertyChanges = entityChangeInfo
                                  .PropertyChanges?
                                  .Select(p => new EntityPropertyChangeEntity(guidGenerator, Id, p, tenantId))
                                  .ToList()
                              ?? new List<EntityPropertyChangeEntity>();


        }
    }
}
