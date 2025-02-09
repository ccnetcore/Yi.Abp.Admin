using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Market;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Managers;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Application.Services;

/// <summary>
/// 交易市场应用服务
/// </summary>
public class MarketService : ApplicationService
{
    private readonly ISqlSugarRepository<MarketGoodsAggregateRoot> _marketGoodsRepository;

    private readonly MarketManager _marketManager;

    public MarketService(ISqlSugarRepository<MarketGoodsAggregateRoot> marketGoodsRepository,
        MarketManager marketManager)
    {
        _marketGoodsRepository = marketGoodsRepository;
        _marketManager = marketManager;
    }

    /// <summary>
    /// 交易市场查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<MarketGetListOutputDto>> GetListAsync(MarketGetListInput input)
    {
        RefAsync<int> total = 0;
        var output = await _marketGoodsRepository._DbQueryable.WhereIF(
                input.StartTime is not null && input.EndTime is not null,
                m => m.CreationTime >= input.StartTime && m.CreationTime <= input.EndTime)

            .LeftJoin<CollectiblesAggregateRoot>((m, c) => m.CollectiblesId == c.Id)
            .OrderByDescending((m, c) => m.CreationTime)
            .Select((m, c) =>
     
                new MarketGetListOutputDto
                {
                    Id = m.Id,
                    CreationTime = m.CreationTime,
                    SellUserId = m.SellUserId,
                    SellNumber = m.SellNumber,
                    UnitPrice=m.UnitPrice,
                    Collectibles = new CollectiblesDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Describe = c.Describe,
                        ValueNumber = c.ValueNumber,
                        Url = c.Url,
                        Rarity =c.Rarity,
                        FindTotal = c.FindTotal,
                        OrderNum = c.OrderNum
                    }
                }
            )

            .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
        return new PagedResultDto<MarketGetListOutputDto>(total, output);
    }


    /// <summary>
    /// 上架商品
    /// </summary>
    [HttpPost("market/shelved")]
    [Authorize]
    public async Task ShelvedGoodsAsync(ShelvedGoodsDto input)
    {
        await _marketManager.ShelvedGoodsAsync(CurrentUser.GetId(), input.CollectiblesId, input.Number, input.Money);
    }

    /// <summary>
    /// 购买商品
    /// </summary>
    [HttpPut("market/purchase")]
    [Authorize]
    public async Task PurchaseGoodsAsync(PurchaseGoodsDto input)
    {
        await _marketManager.PurchaseGoodsAsync(CurrentUser.GetId(),input.MarketGoodsId, input.Number);
    }
}