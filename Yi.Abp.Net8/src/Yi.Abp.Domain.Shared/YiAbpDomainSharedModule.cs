using Volo.Abp.Domain;
using Volo.Abp.SettingManagement;
using Yi.Framework.AuditLogging.Domain.Shared;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.ChatHub.Domain.Shared;
using Yi.Framework.Rbac.Domain.Shared;

namespace Yi.Abp.Domain.Shared
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainSharedModule),
        typeof(YiFrameworkBbsDomainSharedModule),
          typeof(YiFrameworkChatHubDomainSharedModule),
        typeof(YiFrameworkAuditLoggingDomainSharedModule),

        typeof(AbpSettingManagementDomainSharedModule),
        typeof(AbpDddDomainSharedModule))]
    public class YiAbpDomainSharedModule : AbpModule
    {

    }
}