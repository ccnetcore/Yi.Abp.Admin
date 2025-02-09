using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Repositories;

public interface IDiscussLableRepository: ISqlSugarRepository<DiscussLableAggregateRoot,Guid>
{
    /// <summary>
    /// 获取所有分类的字典
    /// </summary>
    /// <returns></returns>
    Task<Dictionary<Guid, DiscussLableCacheItem>> GetDiscussLableCacheMapAsync();
}