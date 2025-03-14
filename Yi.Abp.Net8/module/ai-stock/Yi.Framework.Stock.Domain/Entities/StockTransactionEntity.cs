using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Auditing;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Domain.Entities
{
    /// <summary>
    /// 股票交易记录实体
    /// </summary>
    /// <remarks>
    /// 用于记录用户买入或卖出股票的交易历史
    /// </remarks>
    [SugarTable("Stock_Transaction")]
    public class StockTransactionEntity : Entity<Guid>, IAuditedObject
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        /// <remarks>进行交易的用户</remarks>
        public Guid UserId { get; set; }

        /// <summary>
        /// 股票ID
        /// </summary>
        /// <remarks>交易的股票</remarks>
        public Guid StockId { get; set; }

        /// <summary>
        /// 股票代码
        /// </summary>
        /// <remarks>冗余字段，方便查询</remarks>
        public string StockCode { get; set; } = string.Empty;

        /// <summary>
        /// 股票名称
        /// </summary>
        /// <remarks>冗余字段，方便查询</remarks>
        public string StockName { get; set; } = string.Empty;

        /// <summary>
        /// 交易类型
        /// </summary>
        public TransactionTypeEnum TransactionType { get; set; }

        /// <summary>
        /// 交易价格
        /// </summary>
        /// <remarks>股票的单价</remarks>
        public decimal Price { get; set; }

        /// <summary>
        /// 交易数量
        /// </summary>
        /// <remarks>买入或卖出的股票数量</remarks>
        public int Quantity { get; set; }

        /// <summary>
        /// 交易总额
        /// </summary>
        /// <remarks>价格 × 数量</remarks>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 交易费用
        /// </summary>
        /// <remarks>手续费、佣金等</remarks>
        public decimal Fee { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <remarks>交易发生时间</remarks>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 最后修改者ID
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        public StockTransactionEntity() { }

        public StockTransactionEntity(
            Guid userId,
            Guid stockId,
            string stockCode,
            string stockName,
            TransactionTypeEnum transactionType,
            decimal price,
            int quantity,
            decimal fee = 0)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            StockId = stockId;
            StockCode = stockCode;
            StockName = stockName;
            TransactionType = transactionType;
            Price = price;
            Quantity = quantity;
            TotalAmount = price * quantity;
            Fee = fee;
            CreationTime = DateTime.Now;
        }
    }
} 