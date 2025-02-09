using Volo.Abp.Application.Dtos;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Market;

public class MarketGetListOutputDto:EntityDto<Guid>
{
    /// <summary>
    /// 上架时间
    /// </summary>
    public DateTime CreationTime{ get; set; }
    /// <summary>
    /// 出售者用户id
    /// </summary>
    public Guid SellUserId { get; set; }
    /// <summary>
    /// 出售数量
    /// </summary>
    public int SellNumber{ get; set; }
    
    /// <summary>
    /// 出售单价
    /// </summary>
    public decimal UnitPrice{ get; set; }
    /// <summary>
    /// 藏品
    /// </summary>
    public CollectiblesDto Collectibles{ get; set; }
}