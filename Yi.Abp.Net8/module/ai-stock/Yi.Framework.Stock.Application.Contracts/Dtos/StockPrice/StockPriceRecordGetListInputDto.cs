using System;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockPrice
{
    /// <summary>
    /// 获取股市价格记录的输入DTO
    /// </summary>
    public class StockPriceRecordGetListInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 股票ID
        /// </summary>
        public Guid? StockId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 时间周期类型
        /// </summary>
        public PeriodTypeEnum? PeriodType { get; set; }
    }
} 