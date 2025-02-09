using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Yi.Framework.Core;
using Yi.Framework.WeChat.MiniProgram.Token;

namespace Yi.Framework.WeChat.MiniProgram;

[DependsOn(typeof(YiFrameworkCoreModule),
    typeof(AbpCachingModule))]
public class YiFrameworkWeChatMiniProgramModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var services = context.Services;
        var configuration = context.Services.GetConfiguration();
        Configure<WeChatMiniProgramOptions>(configuration.GetSection("WeChatMiniProgram"));
        services.AddSingleton<IMiniProgramToken, CacheMiniProgramToken>();
    }
}