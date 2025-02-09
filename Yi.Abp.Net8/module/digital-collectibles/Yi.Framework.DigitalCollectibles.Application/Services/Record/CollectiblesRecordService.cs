using Microsoft.AspNetCore.Authorization;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Records;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Entities.Record;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Application.Services.Record;

public class CollectiblesRecordService : ApplicationService
{
    private readonly ISqlSugarRepository<MiningPoolRecordAggregateRoot> _miningPoolRecordRepository;
    private readonly ISqlSugarRepository<MarketRecordAggregateRoot> _marketRecordRepository;
    public CollectiblesRecordService(ISqlSugarRepository<MiningPoolRecordAggregateRoot> miningPoolRecordRepository, ISqlSugarRepository<MarketRecordAggregateRoot> marketRecordRepository)
    {
        _miningPoolRecordRepository = miningPoolRecordRepository;
        _marketRecordRepository = marketRecordRepository;
    }

    /// <summary>
    /// 获取当前用户的挖矿记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<PagedResultDto<MiningPoolRecordDto>> GetMiningPoolAsync(PagedAllResultRequestDto input)
    {
        RefAsync<int> total = 0;
        var userId = CurrentUser.GetId();
        var output =  await _miningPoolRecordRepository._DbQueryable.WhereIF(input.StartTime is not null && input.EndTime is not null,
                x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
            .Where(x => x.UserId == userId)
            .LeftJoin<CollectiblesAggregateRoot>((x, c) => x.CollectiblesId==c.Id)
            .Select((x, c) =>
                new MiningPoolRecordDto
                {
                    Id = x.Id,
                    CreationTime = x.CreationTime,
                    Collectibles = new CollectiblesDto
                    {
                        Id = c.Id,
                        Code = c.Code,
                        Name = c.Name,
                        Describe = c.Describe,
                        ValueNumber = c.ValueNumber,
                        Url = c.Url,
                        Rarity = c.Rarity,
                        FindTotal = c.FindTotal,
                        OrderNum = c.OrderNum
                    }

                }
            )
            .OrderByDescending(x => x.CreationTime)
            .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

        return new PagedResultDto<MiningPoolRecordDto>(total,output);
    }

    /// <summary>
    /// 获取当前用户的交易记录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [Authorize]
    public async Task<PagedResultDto<MarketRecordDto>> GetMarketAsync(PagedAllResultRequestDto input)
    {
        RefAsync<int> total = 0;
        var userId = CurrentUser.GetId();
        var output =  await _marketRecordRepository._DbQueryable.WhereIF(input.StartTime is not null && input.EndTime is not null,
                x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
            
            //交易：是购买和出售，都需要展示
            .Where(x => x.SellUserId == userId||x.BuyId ==userId)

            .LeftJoin<CollectiblesAggregateRoot>((x, c) => x.CollectiblesId==c.Id)
            .Select((x, c) =>
                    new MarketRecordDto
                    {
                        Id = x.Id,
                        CreationTime=x.CreationTime,
                        SellUserId = x.SellUserId,
                        SellNumber = x.SellNumber,
                        RealTotalPrice=x.RealTotalPrice,
                        BuyId = x.BuyId,
                        UnitPrice=x.UnitPrice,
                        Collectibles = new CollectiblesDto()
                        {
                            Id = c.Id,
                            Code = c.Code,
                            Name = c.Name,
                            Describe = c.Describe,
                            ValueNumber = c.ValueNumber,
                            Url = c.Url,
                            Rarity = c.Rarity,
                            FindTotal = c.FindTotal,
                            OrderNum = c.OrderNum
                        },
                    }
            )
            .OrderByDescending(x => x.CreationTime)
            .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
        
        foreach (var dto in output)
        {
            dto.IsBuyer = dto.BuyId == userId;
        }
        
        
        return new PagedResultDto<MarketRecordDto>(total,output);
    }
}