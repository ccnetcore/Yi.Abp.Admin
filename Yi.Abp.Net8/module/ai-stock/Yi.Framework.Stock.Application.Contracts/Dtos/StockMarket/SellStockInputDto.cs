using System;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket
{
    /// <summary>
    /// 卖出股票输入DTO
    /// </summary>
    public class SellStockInputDto
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public Guid StockId { get; set; }
        
        /// <summary>
        /// 卖出数量
        /// </summary>
        public int Quantity { get; set; }
    }
} 