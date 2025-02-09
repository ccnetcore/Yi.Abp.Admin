using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities;

/// <summary>
/// 交易市场货物
/// 用于表示交易市场货物的情况
/// </summary>
[SugarTable("DC_MarketGoods")]
public class MarketGoodsAggregateRoot:FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 出售者用户id
    /// </summary>
    public Guid SellUserId { get; set; }

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
}