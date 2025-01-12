using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Shop;

/// <summary>
/// 商品申请记录表
/// </summary>
[SugarTable("BbsGoodsApply")]
public class BbsGoodsApplyAggregateRoot: AggregateRoot<Guid>, IHasCreationTime
{
    /// <summary>
    /// 商品id
    /// </summary>
    public Guid GoodsId { get; set; }

    /// <summary>
    /// 申请时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 申请人用户id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 联系方式
    /// </summary>
    public string ContactInformation { get; set; }
}