using System;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Stock.Domain.Shared;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockTransaction
{
    /// <summary>
    /// 获取交易记录的输入DTO
    /// </summary>
    public class StockTransactionGetListInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string? StockCode { get; set; }
        
        /// <summary>
        /// 股票名称
        /// </summary>
        public string? StockName { get; set; }
        
        /// <summary>
        /// 交易类型
        /// </summary>
        public TransactionTypeEnum? TransactionType { get; set; }
        
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
} 