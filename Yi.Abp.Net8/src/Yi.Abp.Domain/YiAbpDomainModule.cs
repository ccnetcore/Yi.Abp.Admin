using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Yi.Abp.Domain.Shared;
using Yi.Framework.AuditLogging.Domain;
using Yi.Framework.Mapster;
using Yi.Framework.Rbac.Domain;
using Yi.Framework.SettingManagement.Domain;
using Yi.Framework.TenantManagement.Domain;

namespace Yi.Abp.Domain
{
    [DependsOn(
        typeof(YiAbpDomainSharedModule),

        typeof(YiFrameworkTenantManagementDomainModule),
        typeof(YiFrameworkRbacDomainModule),
        typeof(YiFrameworkAuditLoggingDomainModule),
        typeof(YiFrameworkSettingManagementDomainModule),

        typeof(YiFrameworkMapsterModule),
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule)
        )]
    public class YiAbpDomainModule : AbpModule
    {

    }
}