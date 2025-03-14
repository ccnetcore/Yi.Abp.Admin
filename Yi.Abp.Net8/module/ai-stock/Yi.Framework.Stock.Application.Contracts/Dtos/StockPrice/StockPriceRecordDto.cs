using System;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockPrice
{
    /// <summary>
    /// 股市价格记录DTO
    /// </summary>
    public class StockPriceRecordDto : EntityDto<Guid>
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public Guid StockId { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 当前价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 交易量
        /// </summary>
        public long Volume { get; set; }

        /// <summary>
        /// 交易额
        /// </summary>
        public decimal Turnover { get; set; }

        /// <summary>
        /// 时间周期类型
        /// </summary>
        public PeriodTypeEnum PeriodType { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime { get; set; }
    }
} 