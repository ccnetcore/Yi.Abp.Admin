using Volo.Abp.Application.Dtos;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Records;

public class MarketRecordDto:EntityDto<Guid>
{
    /// <summary>
    /// 当前这条数据是否为购买者，否则为出售者
    /// </summary>
    public bool IsBuyer { get; set; }

    public  DateTime CreationTime { get; set; }
    /// <summary>
    /// 出售数量
    /// </summary>
    public int SellNumber{ get; set; }
    
    /// <summary>
    /// 出售者用户id
    /// </summary>
    public Guid SellUserId { get; set; }

    /// <summary>
    /// 购买者用户id
    /// </summary>
    public Guid BuyId { get; set; }
    
    /// <summary>
    /// 出售单价
    /// </summary>
    public decimal UnitPrice{ get; set; }
    
    /// <summary>
    /// 实际到手价格（扣除税收）
    /// </summary>
    public decimal RealTotalPrice{ get; set; }
    /// <summary>
    /// 交易的藏品
    /// </summary>
    public CollectiblesDto Collectibles { get; set; }
}