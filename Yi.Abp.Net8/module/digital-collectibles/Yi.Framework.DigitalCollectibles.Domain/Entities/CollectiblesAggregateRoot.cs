using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;
using Yi.Framework.Core.Data;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities;

/// <summary>
/// 数字藏品定义表
/// 用于定义数字藏品
/// </summary>
[SugarTable("DC_Collectibles")]
public class CollectiblesAggregateRoot:FullAuditedAggregateRoot<Guid>,IOrderNum
{
    /// <summary>
    /// 藏品编号
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 藏品名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 藏品描述
    /// </summary>
    public string? Describe { get; set; }

    /// <summary>
    /// 价值数
    /// </summary>
    public decimal ValueNumber { get; set; }

    /// <summary>
    /// 藏品地址
    /// </summary>
    public string Url { get; set; }
    
    /// <summary>
    /// 稀有度
    /// </summary>
    public RarityEnum Rarity{ get; set; }

    /// <summary>
    /// 总共出现次数
    /// </summary>
    public int FindTotal { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int OrderNum { get; set; }
}