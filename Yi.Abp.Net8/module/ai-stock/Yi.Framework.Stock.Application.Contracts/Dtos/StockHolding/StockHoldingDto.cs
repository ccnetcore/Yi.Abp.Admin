using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockHolding
{
    /// <summary>
    /// 用户股票持仓DTO
    /// </summary>
    public class StockHoldingDto : EntityDto<Guid>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 股票ID
        /// </summary>
        public Guid StockId { get; set; }
        
        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }
        
        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }
        
        /// <summary>
        /// 持有数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 持仓成本
        /// </summary>
        public decimal CostPrice { get; set; }
        
        /// <summary>
        /// 当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }
        
        /// <summary>
        /// 持仓市值
        /// </summary>
        public decimal MarketValue { get; set; }
        
        /// <summary>
        /// 盈亏金额
        /// </summary>
        public decimal ProfitLoss { get; set; }
        
        /// <summary>
        /// 盈亏百分比
        /// </summary>
        public decimal ProfitLossPercentage { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
} 