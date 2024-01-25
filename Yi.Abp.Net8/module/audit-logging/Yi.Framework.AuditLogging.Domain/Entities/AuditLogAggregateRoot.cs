using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using Yi.Framework.AuditLogging.Domain.Shared.Consts;

namespace Yi.Framework.AuditLogging.Domain.Entities
{
    [DisableAuditing]
    [SugarTable("YiAuditLog")]
    [SugarIndex($"index_{nameof(ExecutionTime)}", nameof(TenantId), OrderByType.Asc,nameof(ExecutionTime), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(ExecutionTime)}_{nameof(UserId)}",nameof(TenantId), OrderByType.Asc, nameof(UserId), OrderByType.Asc, nameof(ExecutionTime), OrderByType.Asc)]
    public class AuditLogAggregateRoot: AggregateRoot<Guid>, IMultiTenant
    {
        public AuditLogAggregateRoot()
        {

        }

        public AuditLogAggregateRoot(
    Guid id,
    string applicationName,
    Guid? tenantId,
    string tenantName,
    Guid? userId,
    string userName,
    DateTime executionTime,
    int executionDuration,
    string clientIpAddress,
    string clientName,
    string clientId,
    string correlationId,
    string browserInfo,
    string httpMethod,
    string url,
    int? httpStatusCode,
    Guid? impersonatorUserId,
    string impersonatorUserName,
    Guid? impersonatorTenantId,
    string impersonatorTenantName,
    ExtraPropertyDictionary extraPropertyDictionary,
    List<EntityChangeEntity> entityChanges,
    List<AuditLogActionEntity> actions,
    string exceptions,
    string comments)
    : base(id)
        {
            ApplicationName = applicationName.Truncate(AuditLogConsts.MaxApplicationNameLength);
            TenantId = tenantId;
            TenantName = tenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
            UserId = userId;
            UserName = userName.Truncate(AuditLogConsts.MaxUserNameLength);
            ExecutionTime = executionTime;
            ExecutionDuration = executionDuration;
            ClientIpAddress = clientIpAddress.Truncate(AuditLogConsts.MaxClientIpAddressLength);
            ClientName = clientName.Truncate(AuditLogConsts.MaxClientNameLength);
            ClientId = clientId.Truncate(AuditLogConsts.MaxClientIdLength);
            CorrelationId = correlationId.Truncate(AuditLogConsts.MaxCorrelationIdLength);
            BrowserInfo = browserInfo.Truncate(AuditLogConsts.MaxBrowserInfoLength);
            HttpMethod = httpMethod.Truncate(AuditLogConsts.MaxHttpMethodLength);
            Url = url.Truncate(AuditLogConsts.MaxUrlLength);
            HttpStatusCode = httpStatusCode;
            ImpersonatorUserId = impersonatorUserId;
            ImpersonatorUserName = impersonatorUserName.Truncate(AuditLogConsts.MaxUserNameLength);
            ImpersonatorTenantId = impersonatorTenantId;
            ImpersonatorTenantName = impersonatorTenantName.Truncate(AuditLogConsts.MaxTenantNameLength);
            ExtraProperties = extraPropertyDictionary;
            EntityChanges = entityChanges;
            Actions = actions;
            Exceptions = exceptions;
            Comments = comments.Truncate(AuditLogConsts.MaxCommentsLength);
        }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        public virtual string? ApplicationName { get; set; }

        public virtual Guid? UserId { get; protected set; }

        public virtual string? UserName { get; protected set; }

        public virtual string? TenantName { get; protected set; }

        public virtual Guid? ImpersonatorUserId { get; protected set; }

        public virtual string? ImpersonatorUserName { get; protected set; }

        public virtual Guid? ImpersonatorTenantId { get; protected set; }

        public virtual string? ImpersonatorTenantName { get; protected set; }

        public virtual DateTime? ExecutionTime { get; protected set; }

        public virtual int? ExecutionDuration { get; protected set; }

        public virtual string? ClientIpAddress { get; protected set; }

        public virtual string? ClientName { get; protected set; }

        public virtual string? ClientId { get; set; }

        public virtual string? CorrelationId { get; set; }

        public virtual string? BrowserInfo { get; protected set; }

        public virtual string? HttpMethod { get; protected set; }

        public virtual string? Url { get; protected set; }

        public virtual string? Exceptions { get; protected set; }

        public virtual string? Comments { get; protected set; }

        public virtual int? HttpStatusCode { get; set; }

        public virtual Guid? TenantId { get; protected set; }

        //导航属性
        [Navigate(NavigateType.OneToMany, nameof(EntityChangeEntity.AuditLogId))]
        public virtual  List<EntityChangeEntity> EntityChanges { get; protected set; }

        //导航属性
        [Navigate(NavigateType.OneToMany, nameof(AuditLogActionEntity.AuditLogId))]
        public  virtual List<AuditLogActionEntity> Actions { get; protected set; }

        [SugarColumn(IsIgnore = true)]
        public override ExtraPropertyDictionary ExtraProperties { get; protected set; }
    }
}
