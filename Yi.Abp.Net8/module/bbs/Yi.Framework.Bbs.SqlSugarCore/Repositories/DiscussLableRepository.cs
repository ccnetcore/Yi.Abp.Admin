using Mapster;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Repositories;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.Framework.Bbs.SqlSugarCore.Repositories;

public class DiscussLableRepository : SqlSugarRepository<DiscussLableAggregateRoot, Guid>, IDiscussLableRepository,
    ITransientDependency
{
    private readonly IDistributedCache<List<DiscussLableCacheItem>> _lableCache;

    public DiscussLableRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider,
        IDistributedCache<List<DiscussLableCacheItem>> lableCache) : base(sugarDbContextProvider)
    {
        _lableCache = lableCache;
    }

    /// <summary>
    /// 获取所有分类的字典
    /// </summary>
    /// <returns></returns>
    public async Task<Dictionary<Guid, DiscussLableCacheItem>> GetDiscussLableCacheMapAsync()
    {
        var cahce = await _lableCache.GetOrAddAsync(DiscussLableConst.DiscussLableCacheKey, async () =>
            {
                var entities = await _DbQueryable.ToListAsync();
                return entities.Adapt<List<DiscussLableCacheItem>>();
            }, () =>
                new DistributedCacheEntryOptions()
                    { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) },hideErrors:true
        );
        return cahce.ToDictionary(x => x.Id);
    }
}