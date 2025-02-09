using Volo.Abp.Domain.Services;
using Yi.Framework.Bbs.Domain.Entities.Shop;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers.Shop;

/// <summary>
/// bbs商城领域服务
/// </summary>
public class BbsShopManager : DomainService
{
    private readonly ISqlSugarRepository<BbsGoodsAggregateRoot> _repository;
    private readonly ISqlSugarRepository<BbsGoodsApplyAggregateRoot> _applyRepository;

    public BbsShopManager(ISqlSugarRepository<BbsGoodsAggregateRoot> repository,
        ISqlSugarRepository<BbsGoodsApplyAggregateRoot> applyRepository)
    {
        _repository = repository;
        _applyRepository = applyRepository;
    }

    /// <summary>
    /// 申请购买商品
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="goodsId"></param>
    /// <param name="contactInformation"></param>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task BuyAsync(Guid userId, Guid goodsId,string contactInformation)
    {
        var goods = await _repository.GetFirstAsync(x => x.Id == goodsId);

        if (goods.GoodsType==GoodsTypeEnum.Apply)
        {
            var count= await _applyRepository.CountAsync(x => x.UserId == userId);
            if (count>=goods.LimitNumber)
            {
                throw new UserFriendlyException("你已经达该商品最大限购次数");
            }

            await _applyRepository.InsertAsync(new BbsGoodsApplyAggregateRoot
            {
                GoodsId = goodsId,
                UserId = userId,
                ContactInformation = contactInformation
            });

            //更新库存
            goods.StockNumber -= 1;
            await _repository.UpdateAsync(goods);
        }
      
    }
}