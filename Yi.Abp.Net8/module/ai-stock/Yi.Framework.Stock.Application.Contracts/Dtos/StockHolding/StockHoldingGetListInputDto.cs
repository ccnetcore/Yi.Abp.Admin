using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockHolding
{
    /// <summary>
    /// 获取用户持仓列表的输入DTO
    /// </summary>
    public class StockHoldingGetListInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string? StockCode { get; set; }
        
        /// <summary>
        /// 股票名称
        /// </summary>
        public string? StockName { get; set; }
    }
} 