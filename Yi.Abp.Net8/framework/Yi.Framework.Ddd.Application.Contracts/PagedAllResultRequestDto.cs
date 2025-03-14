using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Ddd.Application.Contracts
{
    /// <summary>
    /// 分页查询请求DTO，包含时间范围和自定义排序功能
    /// </summary>
    public class PagedAllResultRequestDto : PagedAndSortedResultRequestDto, IPagedAllResultRequestDto
    {
        /// <summary>
        /// 查询开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 查询结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 排序列名
        /// </summary>
        public string? OrderByColumn { get; set; }

        /// <summary>
        /// 排序方向（ascending/descending）
        /// </summary>
        public string? IsAsc { get; set; }

        /// <summary>
        /// 是否为升序排序
        /// </summary>
        public bool IsAscending => string.Equals(IsAsc, "ascending", StringComparison.OrdinalIgnoreCase);

        private string? _sorting;

        /// <summary>
        /// 排序表达式
        /// </summary>
        public override string? Sorting
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(OrderByColumn))
                {
                    return $"{OrderByColumn} {(IsAscending ? "ASC" : "DESC")}";
                }
                return _sorting;
            }
            set => _sorting = value;
        }
    }
}