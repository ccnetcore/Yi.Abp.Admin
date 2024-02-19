using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Framework.Caching.FreeRedis;
using Yi.Framework.Mapster;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Operlog;
using Yi.Framework.Rbac.Domain.Shared;
using Yi.Framework.Rbac.Domain.Shared.Options;

namespace Yi.Framework.Rbac.Domain
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainSharedModule),
        typeof(YiFrameworkCachingFreeRedisModule),

        typeof(AbpAspNetCoreSignalRModule),
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule)
        )]
    public class YiFrameworkRbacDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var service = context.Services;
            var configuration = context.Services.GetConfiguration();
            service.AddControllers(options =>
            {
                options.Filters.Add<PermissionGlobalAttribute>();
                options.Filters.Add<OperLogGlobalAttribute>();
            });

            //配置阿里云短信
            Configure<AliyunOptions>(configuration.GetSection(nameof(AliyunOptions)));
        }
    }
}