using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Stock.Domain.Entities
{
    /// <summary>
    /// 股市聚合根实体
    /// </summary>
    /// <remarks>
    /// 用于定义有哪些公司上架的股市
    /// </remarks>
    [SugarTable("Stock_Market")]
    public class StockMarketAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum, IState
    {
        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

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
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        /// <summary>
        /// 股市代码
        /// </summary>
        public string MarketCode { get; set; } = string.Empty;

        /// <summary>
        /// 股市名称
        /// </summary>
        public string MarketName { get; set; } = string.Empty;

        /// <summary>
        /// 股市描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        public StockMarketAggregateRoot() { }

        public StockMarketAggregateRoot(
            string marketCode,
            string marketName,
            string description = "")
        {
            MarketCode = marketCode;
            MarketName = marketName;
            Description = description;
        }
    }
} 