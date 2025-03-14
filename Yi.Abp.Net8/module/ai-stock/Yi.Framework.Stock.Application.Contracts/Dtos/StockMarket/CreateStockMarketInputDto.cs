using System;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket;
/// <summary>
/// 创建股市输入DTO
/// </summary>
public class CreateStockMarketInputDto
{
  /// <summary>
    /// 股市代码
    /// </summary>
    public string MarketCode { get; set; }
    
    /// <summary>
    /// 股市名称
    /// </summary>
    public string MarketName { get; set; }
    
    /// <summary>
    /// 股市描述
    /// </summary>
    public string Description { get; set; }
}
