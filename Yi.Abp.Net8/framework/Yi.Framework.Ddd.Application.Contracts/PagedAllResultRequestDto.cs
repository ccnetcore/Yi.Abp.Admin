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
    }
}
