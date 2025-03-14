using System;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket
{
    /// <summary>
    /// 买入股票输入DTO
    /// </summary>
    public class BuyStockInputDto
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public Guid StockId { get; set; }
        
        /// <summary>
        /// 买入数量
        /// </summary>
        public int Quantity { get; set; }
    }
} 