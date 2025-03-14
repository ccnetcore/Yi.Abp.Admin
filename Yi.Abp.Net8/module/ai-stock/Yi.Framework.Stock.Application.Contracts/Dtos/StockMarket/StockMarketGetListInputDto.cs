using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket
{
    /// <summary>
    /// 获取股市列表的输入DTO
    /// </summary>
    public class StockMarketGetListInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 股市代码
        /// </summary>
        public string? MarketCode { get; set; }

        /// <summary>
        /// 股市名称
        /// </summary>
        public string? MarketName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool? State { get; set; }
    }
} 