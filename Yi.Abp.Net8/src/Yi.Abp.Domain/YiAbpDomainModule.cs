using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Abp.Domain.Shared;
using Yi.Framework.AuditLogging.Domain;
using Yi.Framework.Bbs.Domain;
using Yi.Framework.ChatHub.Domain;
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
        typeof(YiFrameworkBbsDomainModule),
        typeof(YiFrameworkChatHubDomainModule),
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