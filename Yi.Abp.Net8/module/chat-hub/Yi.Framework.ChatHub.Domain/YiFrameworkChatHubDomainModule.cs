using Volo.Abp.Domain;
using Yi.Framework.Caching.FreeRedis;
using Yi.Framework.ChatHub.Domain.Shared;


namespace Yi.Framework.ChatHub.Domain
{
    [DependsOn(
        typeof(YiFrameworkChatHubDomainSharedModule),
        typeof(YiFrameworkCachingFreeRedisModule),

        typeof(AbpDddDomainModule)
        )]
    public class YiFrameworkChatHubDomainModule : AbpModule
    {
        public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {

        }
    }
}