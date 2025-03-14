using System;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Domain.Shared.Etos
{
    /// <summary>
    /// 股票交易事件数据传输对象
    /// </summary>
    public class StockTransactionEto
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
        /// 交易类型
        /// </summary>
        public TransactionTypeEnum TransactionType { get; set; }
        
        /// <summary>
        /// 交易价格
        /// </summary>
        public decimal Price { get; set; }
        
        /// <summary>
        /// 交易数量
        /// </summary>
        public int Quantity { get; set; }
        
        /// <summary>
        /// 交易总额
        /// </summary>
        public decimal TotalAmount { get; set; }
        
        /// <summary>
        /// 交易费用
        /// </summary>
        public decimal Fee { get; set; }
    }
} 