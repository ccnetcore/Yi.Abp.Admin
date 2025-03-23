using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Yi.Framework.Mapster;
using Yi.Framework.Stock.Domain.Managers;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel.Plugins;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Domain
{
    [DependsOn(
        typeof(YiFrameworkStockDomainSharedModule),
        typeof(YiFrameworkMapsterModule),
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule)
    )]
    public class YiFrameworkStockDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var services = context.Services;
            
            // 配置绑定
            var semanticKernelSection = configuration.GetSection("SemanticKernel");
            services.Configure<SemanticKernelOptions>(configuration.GetSection("SemanticKernel"));
            
            services.AddHttpClient();
#pragma warning disable SKEXP0010
            // 从配置中获取值
            var options = semanticKernelSection.Get<SemanticKernelOptions>();
            //股市优先使用第一个ai模型
            services.AddKernel()
                .AddOpenAIChatCompletion(
                    modelId: options.ModelIds.FirstOrDefault(),
                    endpoint: new Uri(options.Endpoint),
                    apiKey: options.ApiKey);
#pragma warning restore SKEXP0010
            
            // 添加插件
            services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<NewsPlugins>(serviceProvider: sp));
            services.AddSingleton<KernelPlugin>(sp => KernelPluginFactory.CreateFromType<StockPlugins>(serviceProvider: sp));
            
            // 注册NewsManager
            services.AddTransient<NewsManager>();
        }
    }
}