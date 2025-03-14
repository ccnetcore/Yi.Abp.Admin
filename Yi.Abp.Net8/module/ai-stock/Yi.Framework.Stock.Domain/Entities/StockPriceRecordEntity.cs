using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Auditing;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Domain.Entities
{
    /// <summary>
    /// 股票价格记录实体
    /// </summary>
    /// <remarks>
    /// 用于记录每支股票在不同时间点的价格数据，支持趋势分析和图表展示
    /// </remarks>
    [SugarTable("Stock_PriceRecord")]
    public class StockPriceRecordEntity : Entity<Guid>, IHasCreationTime
    {

        /// <summary>
        /// 股票ID
        /// </summary>
        /// <remarks>关联到具体的股票</remarks>
        public Guid StockId { get; set; }

        /// <summary>
        /// 创建时间（审计日志）
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        /// <remarks>价格记录的实际时间点</remarks>
        public DateTime RecordTime { get; set; }

        /// <summary>
        /// 当前价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 交易量
        /// </summary>
        /// <remarks>该时间段内的交易股数</remarks>
        public long Volume { get; set; }

        /// <summary>
        /// 交易额
        /// </summary>
        /// <remarks>该时间段内的交易金额</remarks>
        public decimal Turnover { get; set; }

        /// <summary>
        /// 时间周期类型
        /// </summary>
        /// <remarks>
        /// 记录的时间周期类型：分钟、小时、日、周、月等
        /// </remarks>
        public PeriodTypeEnum PeriodType { get; set; }

        public StockPriceRecordEntity() { }

        public StockPriceRecordEntity(
            Guid stockId, 
            decimal currentPrice, 
            long volume = 0,
            decimal turnover = 0,
            PeriodTypeEnum periodType = PeriodTypeEnum.Day)
        {
            StockId = stockId;
            CreationTime = DateTime.Now;
            RecordTime = DateTime.Now;
            CurrentPrice = currentPrice;
            Volume = volume;
            Turnover = turnover;
            PeriodType = periodType;
        }
    }
}