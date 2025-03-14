using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockNews
{
    /// <summary>
    /// 股市新闻DTO
    /// </summary>
    public class StockNewsDto : EntityDto<Guid>
    {
        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// 新闻内容
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }
        
        /// <summary>
        /// 新闻来源
        /// </summary>
        public string Source { get; set; }
        
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
        
        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNum { get; set; }
    }
} 