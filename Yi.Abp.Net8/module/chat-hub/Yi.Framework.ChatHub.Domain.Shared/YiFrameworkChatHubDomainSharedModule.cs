using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Domain;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.ChatHub.Domain.Shared.Options;

namespace Yi.Framework.ChatHub.Domain.Shared
{
    [DependsOn(
        typeof(AbpDddDomainSharedModule),

        typeof(YiFrameworkBbsDomainSharedModule))]
    public class YiFrameworkChatHubDomainSharedModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<AiOptions>(configuration.GetSection("AiOptions"));
        }
    }
}