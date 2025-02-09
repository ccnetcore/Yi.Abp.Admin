using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Shop;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Shop;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Managers.Shop;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Shop;

/// <summary>
/// bbs商城服务
/// </summary>
public class BbsShopService : ApplicationService
{
    private readonly ISqlSugarRepository<BbsGoodsAggregateRoot> _repository;
    private readonly ISqlSugarRepository<BbsGoodsApplyAggregateRoot> _applyRepository;
    private readonly BbsShopManager _bbsShopManager;
    private readonly ISqlSugarRepository<BbsUserExtraInfoEntity> _bbsUserRepository;
    private ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();

    public BbsShopService(ISqlSugarRepository<BbsGoodsAggregateRoot> repository,
        ISqlSugarRepository<BbsGoodsApplyAggregateRoot> applyRepository, BbsShopManager bbsShopManager,
        ISqlSugarRepository<BbsUserExtraInfoEntity> bbsUserRepository)
    {
        _repository = repository;
        _applyRepository = applyRepository;
        _bbsShopManager = bbsShopManager;
        _bbsUserRepository = bbsUserRepository;
    }

    //商城列表
    [Authorize]
    public async Task<PagedResultDto<ShopGetListOutput>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var output = new List<ShopGetListOutput>();
        var userId = CurrentUser.GetId();
        RefAsync<int> total = 0;
        var entities = await _repository._DbQueryable
            .Where(x => x.EndTime > DateTime.Now)
            .OrderBy(x => x.OrderNum)
            .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
        var applyEntities = await _applyRepository.GetListAsync(x => x.UserId == userId);

        foreach (var entity in entities)
        {
            var dto = entity.Adapt<ShopGetListOutput>();
            if (entity.GoodsType == GoodsTypeEnum.Apply)
            {
                //大于限购数量
                if (applyEntities.Count(x => x.GoodsId == entity.Id) >= entity.LimitNumber)
                {
                    dto.IsLimit = true;
                }
            }

            output.Add(dto);
        }

        return new PagedResultDto<ShopGetListOutput>(total, output);
    }

    /// <summary>
    /// 购买商品
    /// </summary>
    [Authorize]
    public async Task PostBuyAsync([FromBody] BuyShopInputDto input)
    {
        var userId = CurrentUser.GetId();
        await _bbsShopManager.BuyAsync(userId, input.GoodsId, input.ContactInformation);
    }

    /// <summary>
    /// 获取该用户汇总信息（钱钱、积分、价值）
    /// </summary>
    [Authorize]
    public async Task<BbsShopAccountDto> GetAccountAsync()
    {
        var userId = CurrentUser.GetId();
        var output = new BbsShopAccountDto();
        var money = await _bbsUserRepository._DbQueryable.Where(x => x.UserId == userId).Select(x => x.Money)
            .FirstAsync();
        var eto = new SetAccountInfoEto(userId);
        await LocalEventBus.PublishAsync(eto, false);
        //钱钱累加
        output.Money =money+ eto.Money;
        output.Points = eto.Points;
        output.Value = eto.Value;
        return output;
    }
}