using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectMapping;
using Yi.Framework.Core;
using Mapster;

namespace Yi.Framework.Mapster
{
    /// <summary>
    /// Yi框架Mapster模块
    /// 用于配置和注册Mapster相关服务
    /// </summary>
    [DependsOn(
        typeof(YiFrameworkCoreModule),
        typeof(AbpObjectMappingModule)
    )]
    public class YiFrameworkMapsterModule : AbpModule
    {
        /// <summary>
        /// 配置服务
        /// </summary>
        /// <param name="context">服务配置上下文</param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            // 扫描并注册所有映射配置 
            TypeAdapterConfig.GlobalSettings.Scan(AppDomain.CurrentDomain.GetAssemblies());
            // 注册Mapster相关服务
            services.AddTransient<IAutoObjectMappingProvider, MapsterAutoObjectMappingProvider>();
            services.AddTransient<IObjectMapper, MapsterObjectMapper>();
        }
    }
}