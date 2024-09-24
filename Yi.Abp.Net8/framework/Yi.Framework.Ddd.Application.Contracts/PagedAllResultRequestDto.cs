using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Ddd.Application.Contracts
{
    public class PagedAllResultRequestDto : PagedAndSortedResultRequestDto, IPagedAllResultRequestDto
    {
        /// <summary>
        /// 查询开始时间条件
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 查询结束时间条件
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 排序列名，字段名对应前端
        /// </summary>
        public string? OrderByColumn { get; set; }

        /// <summary>
        /// 是否顺序，字段名对应前端
        /// </summary>
        public string? IsAsc { get; set; }

        /// <summary>
        /// 是否顺序
        /// </summary>
        public bool CanAsc => IsAsc?.ToLower() == "ascending" ? true : false;

        private string _sorting;

        //排序引用
        public new string? Sorting
        {
            get
            {
                if (!OrderByColumn.IsNullOrWhiteSpace())
                {
                    return $"{OrderByColumn} {(CanAsc ? "ASC" : "DESC")}";
                }
                else
                {
                    return _sorting;
                }
            }
            set => _sorting = value;
        }
    }
}