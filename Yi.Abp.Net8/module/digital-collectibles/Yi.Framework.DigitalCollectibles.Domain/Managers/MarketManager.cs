using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Settings;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.Managers;

/// <summary>
/// 市场领域服务
/// 处理交易市场相关业务，例如交易等
/// </summary>
public class MarketManager : DomainService
{
    private readonly ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> _collectiblesUserStoreRepository;

    private readonly ISqlSugarRepository<MarketGoodsAggregateRoot> _marketGoodsRepository;

    private readonly ILocalEventBus _localEventBus;
    private readonly ISettingProvider _settingProvider;

    public MarketManager(ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> collectiblesUserStoreRepository,
        ISqlSugarRepository<MarketGoodsAggregateRoot> marketGoodsRepository, ILocalEventBus localEventBus,
        ISettingProvider settingProvider)
    {
        _collectiblesUserStoreRepository = collectiblesUserStoreRepository;
        _marketGoodsRepository = marketGoodsRepository;
        _localEventBus = localEventBus;
        _settingProvider = settingProvider;
    }


    /// <summary>
    /// 商品自动流拍，自动下架
    /// </summary>
    public async Task AutoPassInGoodsAsync()
    {
        var autoPassInGoodsDay = (await _settingProvider.GetOrNullAsync("AutoPassInGoodsDay")).To<int>();
        //下架3天前的商品
        var endTine = DateTime.Now.AddDays(-autoPassInGoodsDay);
        //获取所有需要下架商品
        var marketGoods = await _marketGoodsRepository._DbQueryable.Where(x => x.CreationTime <= endTine).ToListAsync();

        //单独处理每一个下架的商品
        foreach (var goods in marketGoods)
        {
            //用户库存
            var userStore = await _collectiblesUserStoreRepository._DbQueryable
                .Where(x=>x.UserId==goods.SellUserId)
                .Where(x => x.CollectiblesId == goods.CollectiblesId)
                .Where(x => x.IsAtMarketing == true).FirstAsync();
            if (userStore is not null)
            {
                userStore.IsAtMarketing = false;
                await _collectiblesUserStoreRepository.UpdateAsync(userStore);
                //删除商品
                await _marketGoodsRepository.DeleteByIdAsync(goods.Id);
            }
        }
    }

    /// <summary>
    /// 上架货物
    /// </summary>
    /// <param name="userId">上架者</param>
    /// <param name="collectiblesId">上架的收藏品id</param>
    /// <param name="number">上架数量</param>
    /// <param name="money">上架单价</param>
    /// <returns></returns>
    public async Task ShelvedGoodsAsync(Guid userId, Guid collectiblesId, int number, decimal money)
    {
        if (number <= 0)
        {
            throw new UserFriendlyException("上架藏品数量需大于0");
        }

        var collectiblesList = await _collectiblesUserStoreRepository._DbQueryable
            .Where(x => x.UserId == userId)
            .Where(x => x.IsAtMarketing == false)
            .Where(x => x.CollectiblesId == collectiblesId).ToListAsync();
        if (collectiblesList.Count < number)
        {
            throw new UserFriendlyException($"您的非上架该藏品不足{number}个，上架失败");
        }

        //上架收藏品
        var shelvedcollectibles = collectiblesList.Take(number);
        foreach (var store in shelvedcollectibles)
        {
            store.ShelvedMarket();
        }

        await _collectiblesUserStoreRepository.UpdateRangeAsync(shelvedcollectibles.ToList());

        await _marketGoodsRepository.InsertAsync(new MarketGoodsAggregateRoot
        {
            SellUserId = userId,
            CollectiblesId = collectiblesId,
            SellNumber = number,
            UnitPrice = money
        });
    }

    /// <summary>
    /// 购买商品
    /// </summary>
    /// <param name="userId">购买者用户</param>
    /// <param name="marketGoodsId">商品id</param>
    /// <param name="number">购买数量</param>
    /// <returns></returns>
    public async Task PurchaseGoodsAsync(Guid userId, Guid marketGoodsId, int number)
    {
        if (number <= 0)
        {
            throw new UserFriendlyException("交易藏品数量需大于0");
        }

        //1-市场扣减或者关闭该商品
        //2-出售者新增钱，购买者扣钱
        //3-出售者删除对应库存，购买者新增对应库存
        var marketGoods = await _marketGoodsRepository.GetAsync(x => x.Id == marketGoodsId);

        if (marketGoods is null)
        {
            throw new UserFriendlyException($"交易失败，当前交易市场不存在该商品");
        }
        
        //1-市场扣减或者关闭该商品
        if (marketGoods.SellNumber == number)
        {
            await _marketGoodsRepository.DeleteAsync(x => x.Id == marketGoodsId);
        }
        else if (marketGoods.SellNumber >= number)
        {
            marketGoods.SellNumber -= number;
            await _marketGoodsRepository.UpdateAsync(marketGoods);
        }
        else
        {
            throw new UserFriendlyException($"交易失败，当前交易市场库存不足");
        }

        //2-出售者新增钱，购买者扣钱
        //发布一个其他领域的事件-购买者扣钱
        await _localEventBus.PublishAsync(
            new MoneyChangeEventArgs() { UserId = userId, Number = -(number * marketGoods.UnitPrice) }, false);
        //发布一个其他领域的事件-出售者加钱，同时扣税
        var marketTaxRate = decimal.Parse(await _settingProvider.GetOrNullAsync("MarketTaxRate"));
        //价格*扣减税
        var realTotalPrice = (number * marketGoods.UnitPrice) * (1 - marketTaxRate);
        //出售者实际加钱
        await _localEventBus.PublishAsync(
            new MoneyChangeEventArgs() { UserId = marketGoods.SellUserId, Number = realTotalPrice }, false);

        //3-出售者删除对应库存，购买者新增对应库存(只需更改用户者即可)
        var collectiblesList = await _collectiblesUserStoreRepository._DbQueryable.Where(x => x.IsAtMarketing == true)
            .Where(x => x.UserId == marketGoods.SellUserId)
            .Where(x => x.CollectiblesId == marketGoods.CollectiblesId)
            .ToListAsync();
        if (collectiblesList.Count < number)
        {
            throw new UserFriendlyException($"交易失败，当前出售者库存不足");
        }

        var updateStore = collectiblesList.Take(number).ToList();
        foreach (var userStore in updateStore)
        {
            userStore.PurchaseMarket(userId);
        }

        await _collectiblesUserStoreRepository.UpdateRangeAsync(updateStore);

        //发布一个成功交易事件
        await _localEventBus.PublishAsync(new SuccessMarketEto
        {
            SellUserId = marketGoods.SellUserId,
            BuyId = userId,
            CollectiblesId = marketGoods.CollectiblesId,
            SellNumber = marketGoods.SellNumber,
            UnitPrice = marketGoods.UnitPrice,
            RealTotalPrice = realTotalPrice
        }, false);
    }
}