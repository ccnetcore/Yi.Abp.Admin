using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Entities.Record;
using Yi.Framework.DigitalCollectibles.Domain.Managers;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.EventHandlers;

/// <summary>
/// 临时账号绑定到正式账号，价值、积分 （累加）
/// </summary>
public class BindAccountForCollectiblesEventHandler : ILocalEventHandler<BindAccountEto>, ITransientDependency
{
    private readonly InvitationCodeManager _invitationCodeManager;

    private readonly ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> _collectiblesUserStoreRepository;
    
    
    private readonly ISqlSugarRepository<MiningPoolRecordAggregateRoot> _miningPoolRecordRepository;
    private readonly ISqlSugarRepository<MarketRecordAggregateRoot> _marketRecordRepository;
    private readonly ISqlSugarRepository<MarketGoodsAggregateRoot> _marketGoodsRepository;
    public BindAccountForCollectiblesEventHandler(InvitationCodeManager invitationCodeManager, ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> collectiblesUserStoreRepository, ISqlSugarRepository<MiningPoolRecordAggregateRoot> miningPoolRecordRepository, ISqlSugarRepository<MarketRecordAggregateRoot> marketRecordRepository, ISqlSugarRepository<MarketGoodsAggregateRoot> marketGoodsRepository)
    {
        _invitationCodeManager = invitationCodeManager;
        _collectiblesUserStoreRepository = collectiblesUserStoreRepository;
        _miningPoolRecordRepository = miningPoolRecordRepository;
        _marketRecordRepository = marketRecordRepository;
        _marketGoodsRepository = marketGoodsRepository;
    }

    public async Task HandleEventAsync(BindAccountEto eventData)
    {
        var oldEntity = await _invitationCodeManager.TryGetOrAddAsync(eventData.OldUserId);
        var newEntity = await _invitationCodeManager.TryGetOrAddAsync(eventData.NewUserId);

        newEntity.PointsNumber += oldEntity.PointsNumber;

        //临时账号邀请了，老的账号没有邀请，覆盖
        if (newEntity.IsInvited == false && oldEntity.IsInvited == true)
        {
            newEntity.IsInvited = true;
        }

        await _invitationCodeManager._repository.UpdateAsync(newEntity);
        
        //藏品转移
        var oldUserStore= await  _collectiblesUserStoreRepository.GetListAsync(x => x.UserId == eventData.OldUserId);
        if (oldUserStore.Count>0)
        {
            oldUserStore?.ForEach(x=>x.UserId=eventData.NewUserId);
            await _collectiblesUserStoreRepository.UpdateRangeAsync(oldUserStore);
        }
       
        //挖矿记录转移
        var miningPoolRecord= await  _miningPoolRecordRepository.GetListAsync(x => x.UserId == eventData.OldUserId);
        if (miningPoolRecord.Count>0)
        {
            miningPoolRecord?.ForEach(x=>x.UserId=eventData.NewUserId);
            await _miningPoolRecordRepository.UpdateRangeAsync(miningPoolRecord);
        }

        //交易记录转移
        var marketRecord= await  _marketRecordRepository.GetListAsync(x => x.SellUserId == eventData.OldUserId||x.BuyId==eventData.OldUserId);
        if (marketRecord.Count>0)
        {
            marketRecord?.ForEach(x =>
            {
                if (x.SellUserId == eventData.OldUserId)
                {
                    x.SellUserId = eventData.NewUserId;
                }

                if (x.BuyId == eventData.OldUserId)
                {
                    x.BuyId = eventData.NewUserId;
                }
            });
            await _marketRecordRepository.UpdateRangeAsync(marketRecord);
        }
        
        
        //商城物品交易转移
        var marketGoods= await  _marketGoodsRepository.GetListAsync(x => x.SellUserId == eventData.OldUserId);
        if (marketGoods.Count>0)
        {
            marketGoods?.ForEach(x=>x.SellUserId=eventData.NewUserId);
            await _marketGoodsRepository.UpdateRangeAsync(marketGoods);
        }
    }
}