using Volo.Abp.Domain;
using Yi.Framework.Bbs.Domain.Shared;

namespace Yi.Framework.ChatHub.Domain.Shared
{
    [DependsOn(
        typeof(AbpDddDomainSharedModule),
        
        typeof(YiFrameworkBbsDomainSharedModule))]
    public class YiFrameworkChatHubDomainSharedModule : AbpModule
    {

    }
}