using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.Users;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Caches;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.Managers;
/// <summary>
/// 藏品领域服务
/// 用于管理用户的藏品库存、藏品的业务逻辑
/// </summary>
public class CollectiblesManager:DomainService
{
    private readonly ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> _collectiblesUserStoreRepository;
    private readonly ISqlSugarRepository<CollectiblesAggregateRoot> _collectiblesRepository;
    private readonly IDistributedCache<List<CollectiblesValueCacheItem>> _distributedCache;
    public CollectiblesManager(ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> collectiblesUserStoreRepository, ISqlSugarRepository<CollectiblesAggregateRoot> collectiblesRepository, IDistributedCache<List<CollectiblesValueCacheItem>> distributedCache)
    {
        _collectiblesUserStoreRepository = collectiblesUserStoreRepository;
        _collectiblesRepository = collectiblesRepository;
        _distributedCache = distributedCache;
    }

    /// <summary>
    /// 获取某个用户的价值
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<decimal> GetAccountValueAsync(Guid userId)
    {
        var collectiblesList = await _collectiblesUserStoreRepository._DbQueryable
            .Where(store => store.UserId == userId)
            .LeftJoin<CollectiblesAggregateRoot>((store, c) => store.CollectiblesId == c.Id)
            .Select((store, c) =>
                new
                {
                    c.Id,
                    c.ValueNumber
                }
            ).ToListAsync();
       var  totalValue=ComputeValue(collectiblesList.Select(x=> (x.Id,x.ValueNumber)).ToList());
        return totalValue;
    }

    //计算价值，需要每个藏品的唯一值和藏品的价值即可
    private decimal ComputeValue(List<(Guid collectiblesId,decimal valueNumber)> data)
    {
        var groupBy = data.GroupBy(x => x.collectiblesId);
        decimal totalValue = 0;

        //首个价值百分之百，后续每个只有百分之40，最大10个
        foreach (var groupByItem in groupBy)
        {
            foreach (var item in groupByItem.Select((value, index) => new { value, index }))
            {
                
                if (item.index == 0)
                {
                    totalValue += item.value.valueNumber;
                }
                else if (item.index == 10)
                {
                    //到第11个，直接跳出循环
                    break;
                }
                else
                {
                    totalValue += item.value.valueNumber * 0.4m;
                }
            }
        }

        return totalValue;
    }

/// <summary>
/// 获取全量的排行榜
/// </summary>
/// <returns></returns>
    public async Task<List<CollectiblesValueCacheItem>> GetAllAccountValueByCacheAsync()
    {
        return  await _distributedCache.GetOrAddAsync("AllAccountValue", async () => await GetAccountValueAsync(),
            () => new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
    }
    
    private async Task<List<CollectiblesValueCacheItem>> GetAccountValueAsync()
    {
        var output = new List<CollectiblesValueCacheItem>();
        //获取全部用户的库存
        var allStore=  await _collectiblesUserStoreRepository._DbQueryable.ToListAsync();
        //获取全部藏品
        var allCollectiblesDic= (await _collectiblesRepository._DbQueryable.ToListAsync()).ToDictionary(x=>x.Id,y=>y.ValueNumber);
        
        //根据用户分组
        var userGroup=  allStore.GroupBy(x => x.UserId);
        //每个用户进行计算价值
        foreach (var item in userGroup)
        {
           var value= ComputeValue(item.Select(x => (x.CollectiblesId, allCollectiblesDic[x.CollectiblesId])).ToList());
           output.Add(new CollectiblesValueCacheItem
           {
               UserId = item.Key,
               Value = value
           });
        }

        return output;
    }
    
    
    /// <summary>
    /// 全量更新藏品价值
    /// </summary>
    /// <returns></returns>
    public async Task UpdateAllValueAsync()
    {
        //全部藏品
        var collectibles = await _collectiblesRepository.GetListAsync();
        foreach (var item in collectibles)
        {
            var defaultValue=  item.Rarity.GetDefaultValue();
            //计算实际的百分比
            var realValueRate=  CalculateValue(item.FindTotal);

            var realValue = Math.Round(defaultValue * realValueRate);
            item.ValueNumber = realValue.To<decimal>();
        }

        await _collectiblesRepository.UpdateRangeAsync(collectibles);
    }

    /// <summary>
    /// 价值计算公式
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    private double CalculateValue(double x)
    {
        return 0.1 + 0.9 * Math.Exp(-0.00824 * (x - 1));
    }
}