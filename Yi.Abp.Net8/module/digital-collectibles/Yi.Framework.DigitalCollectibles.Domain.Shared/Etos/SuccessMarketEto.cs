namespace Yi.Framework.DigitalCollectibles.Domain.Shared.Etos;

/// <summary>
/// 成功交易eto
/// </summary>
public class SuccessMarketEto
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