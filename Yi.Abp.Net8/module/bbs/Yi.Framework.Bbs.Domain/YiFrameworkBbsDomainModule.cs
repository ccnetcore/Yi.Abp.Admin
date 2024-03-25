using System.Collections.Generic;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            var logger = services.GetRequiredService<ILogger<YiFrameworkBbsDomainModule>>();
            logger.LogInformation("正在初始化【BBS-等级数据】......");
            var levelRepository = services.GetRequiredService<IRepository<LevelEntity>>();
            var levelCache = services.GetRequiredService<IDistributedCache<List<LevelCacheItem>>>();
            var cacheItem = (await levelRepository.GetListAsync()).Adapt<List<LevelCacheItem>>();
            await levelCache.SetAsync(LevelConst.LevelCacheKey, cacheItem);
            logger.LogInformation("已完成初始化【BBS-等级数据】");
        }
    }
}