using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Shop;

public class ShopGetListOutput:EntityDto<Guid>
{
    /// <summary>
    /// 上架时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 商品类型
    /// </summary>
    public GoodsTypeEnum GoodsType{ get; set; }
    
    /// <summary>
    /// 下架时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 每人限购数量
    /// </summary>
    public int LimitNumber { get; set; }
    
    /// <summary>
    /// 当前库存数量
    /// </summary>
    public int StockNumber { get; set; }

    /// <summary>
    /// 商品图片url
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Describe { get; set; }

    /// <summary>
    /// 编号
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 所需钱钱
    /// </summary>
    public decimal NeedMoney { get; set; }
    
    /// <summary>
    /// 所需价值
    /// </summary>
    public decimal NeedValue { get; set; }
    
    /// <summary>
    /// 所需积分
    /// </summary>
    public decimal NeedPoints { get; set; }

    public int OrderNum { get; set; }
    
    /// <summary>
    /// 是否已限制
    /// </summary>
    public bool IsLimit { get; set; }
}