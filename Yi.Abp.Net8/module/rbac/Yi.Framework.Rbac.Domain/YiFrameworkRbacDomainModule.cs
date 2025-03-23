using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Domain;
using Volo.Abp.Imaging;
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
        typeof(AbpCachingModule),
        typeof(AbpImagingImageSharpModule),
        typeof(AbpDistributedLockingModule)
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
            
            //分布式锁,需要redis
            if (configuration.GetSection("Redis").GetValue<bool>("IsEnabled"))
            {
                context.Services.AddSingleton<IDistributedLockProvider>(sp =>
                {
                    var connection = ConnectionMultiplexer
                        .Connect(configuration["Redis:Configuration"]);
                    return new 
                        RedisDistributedSynchronizationProvider(connection.GetDatabase());
                });
            }

        }
    }
}