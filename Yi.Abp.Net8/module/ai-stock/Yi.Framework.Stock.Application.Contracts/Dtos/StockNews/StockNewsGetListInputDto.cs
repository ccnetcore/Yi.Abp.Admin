using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockNews
{
    /// <summary>
    /// 获取股市新闻列表的输入DTO
    /// </summary>
    public class StockNewsGetListInputDto : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 新闻标题关键词
        /// </summary>
        public string? Title { get; set; }
        
        /// <summary>
        /// 新闻来源
        /// </summary>
        public string? Source { get; set; }
        
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        
        /// <summary>
        /// 是否只显示最近10天的新闻
        /// </summary>
        /// <remarks>默认为true，查询最近10天的新闻</remarks>
        public bool IsRecent { get; set; } = true;
    }
} 