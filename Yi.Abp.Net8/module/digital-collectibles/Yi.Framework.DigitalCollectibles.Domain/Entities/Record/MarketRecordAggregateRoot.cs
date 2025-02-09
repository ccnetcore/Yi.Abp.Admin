using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities.Record;

/// <summary>
/// 交易记录
/// </summary>
[SugarTable("DC_MarketRecord")]
public class MarketRecordAggregateRoot:FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 出售者用户id
    /// </summary>
    public Guid SellUserId { get; set; }

    /// <summary>
    /// 购买者用户id
    /// </summary>
    public Guid BuyId { get; set; }

    /// <summary>
    /// 藏品id
    /// </summary>
    public Guid CollectiblesId { get; set; }

    /// <summary>
    /// 出售数量
    /// </summary>
    public int SellNumber{ get; set; }
    
    /// <summary>
    /// 出售单价
    /// </summary>
    public decimal UnitPrice{ get; set; }
    
    /// <summary>
    /// 实际到手价格（扣除税收）
    /// </summary>
    public decimal RealTotalPrice{ get; set; }
}