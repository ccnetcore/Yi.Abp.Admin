using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Stock.Domain.Entities
{
    /// <summary>
    /// 股市新闻聚合根实体
    /// </summary>
    /// <remarks>
    /// 用于记录影响股市波动的新闻事件
    /// </remarks>
    [SugarTable("Stock_News")]
    public class StockNewsAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum
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
        /// 创建者ID
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 最后修改者ID
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 新闻标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 新闻内容
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 新闻来源
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// 新闻摘要
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        public StockNewsAggregateRoot() { }

        public StockNewsAggregateRoot(
            string title,
            string content,
            string source = "")
        {
            Title = title;
            Content = content;
            Source = source;
            PublishTime = DateTime.Now;
        }

    }
} 