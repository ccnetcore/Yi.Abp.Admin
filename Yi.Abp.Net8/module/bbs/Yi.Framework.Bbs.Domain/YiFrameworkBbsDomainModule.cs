using System.Collections.Generic;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain;

namespace Yi.Framework.Bbs.Domain
{
    [DependsOn(
        typeof(YiFrameworkBbsDomainSharedModule),

        typeof(YiFrameworkRbacDomainModule)
        )]
    public class YiFrameworkBbsDomainModule : AbpModule
    {
        public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            //加载等级缓存
            var services = context.ServiceProvider;
            var levelRepository = services.GetRequiredService<IRepository<LevelEntity>>();
            var levelCache = services.GetRequiredService<IDistributedCache<List<LevelCacheItem>>>();
            var cacheItem = (await levelRepository.GetListAsync()).Adapt<List<LevelCacheItem>>();
            await levelCache.SetAsync(LevelConst.LevelCacheKey, cacheItem);
        }
    }
}