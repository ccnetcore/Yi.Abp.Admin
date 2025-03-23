using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Volo.Abp.Domain;
using Yi.Framework.Caching.FreeRedis;
using Yi.Framework.ChatHub.Domain.Shared;
using Yi.Framework.Core.Options;


namespace Yi.Framework.ChatHub.Domain
{
    [DependsOn(
        typeof(YiFrameworkChatHubDomainSharedModule),
        typeof(YiFrameworkCachingFreeRedisModule),

        typeof(AbpDddDomainModule)
        )]
    public class YiFrameworkChatHubDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var services = context.Services;
            
            // 配置绑定
            var semanticKernelSection = configuration.GetSection("SemanticKernel");
            services.Configure<SemanticKernelOptions>(configuration.GetSection("SemanticKernel"));
#pragma warning disable SKEXP0010
            // 从配置中获取值
            var options = semanticKernelSection.Get<SemanticKernelOptions>();
            foreach (var optionsModelId in options.ModelIds)
            {
                services.AddKernel()
                    .AddOpenAIChatCompletion(
                        serviceId: optionsModelId,
                        modelId: optionsModelId,
                        endpoint: new Uri(options.Endpoint),
                        apiKey: options.ApiKey); 
            }
#pragma warning restore SKEXP0010
        }
    }
}