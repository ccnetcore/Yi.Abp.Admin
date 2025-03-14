using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Stock.Domain.Entities
{
    /// <summary>
    /// 用户股票持仓聚合根
    /// </summary>
    /// <remarks>
    /// 记录用户持有的股票数量和相关信息
    /// </remarks>
    [SugarTable("Stock_Holding")]
    public class StockHoldingAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
    {

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        /// <remarks>关联到持有股票的用户</remarks>
        public Guid UserId { get; set; }

        /// <summary>
        /// 股票ID
        /// </summary>
        /// <remarks>关联到具体的股票</remarks>
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
        /// 持有数量
        /// </summary>
        /// <remarks>用户持有的股票数量</remarks>
        public int Quantity { get; set; }

        /// <summary>
        /// 平均成本价
        /// </summary>
        /// <remarks>用户购买这些股票的平均成本价</remarks>
        public decimal AverageCostPrice { get; set; }

        /// <summary>
        /// 持仓成本
        /// </summary>
        /// <remarks>总投入成本 = 平均成本价 * 持有数量</remarks>
        [SugarColumn(IsIgnore = true)]
        public decimal TotalCost => AverageCostPrice * Quantity;

        public StockHoldingAggregateRoot() { }

        public StockHoldingAggregateRoot(
            Guid userId,
            Guid stockId,
            string stockCode,
            string stockName,
            int quantity,
            decimal averageCostPrice)
        {
            UserId = userId;
            StockId = stockId;
            StockCode = stockCode;
            StockName = stockName;
            Quantity = quantity;
            AverageCostPrice = averageCostPrice;
        }

        /// <summary>
        /// 增加持仓数量
        /// </summary>
        /// <param name="quantity">增加的数量</param>
        /// <param name="price">本次购买价格</param>
        public void AddQuantity(int quantity, decimal price)
        {
            if (quantity <= 0)
                throw new ArgumentException("增加的数量必须大于0");

            // 计算新的平均成本价
            decimal totalCost = AverageCostPrice * Quantity + price * quantity;
            Quantity += quantity;
            AverageCostPrice = totalCost / Quantity;
        }

        /// <summary>
        /// 减少持仓数量
        /// </summary>
        /// <param name="quantity">减少的数量</param>
        public void ReduceQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("减少的数量必须大于0");

            if (quantity > Quantity)
                throw new ArgumentException("减少的数量不能大于持有数量");

            Quantity -= quantity;

            // 如果数量为0，标记为删除
            if (Quantity == 0)
            {
                IsDeleted = true;
            }
        }
    }
}