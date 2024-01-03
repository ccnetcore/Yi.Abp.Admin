using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;
using Yi.Framework.Core;

namespace Yi.Framework.Mapster
{
    [DependsOn(typeof(YiFrameworkCoreModule),

        typeof(AbpObjectMappingModule)
        )]
    public class YiFrameworkMapsterModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddTransient<IAutoObjectMappingProvider, MapsterAutoObjectMappingProvider>();
        }
    }
}