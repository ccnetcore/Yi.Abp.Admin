using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yi.Abp.Tool.Domain.Shared;
using Yi.Abp.Tool.Domain.Shared.Options;

namespace Yi.Abp.Tool.Domain
{
    [DependsOn(typeof(YiAbpToolDomainSharedModule))]
    public class YiAbpToolDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            Configure<ToolOptions>(configuration.GetSection("ToolOptions"));
            var toolOptions = new ToolOptions();
            configuration.GetSection("ToolOptions").Bind(toolOptions);
            if (!Directory.Exists(toolOptions.TempDirPath))
            {
                Directory.CreateDirectory(toolOptions.TempDirPath);
            }
            
        }
    }
}
