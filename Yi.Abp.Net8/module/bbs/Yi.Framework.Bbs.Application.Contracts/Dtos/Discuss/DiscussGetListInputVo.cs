using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss
{
    public class DiscussGetListInputVo : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 创建者的用户名
        /// </summary>
        public string? UserName { get; set; }
        public Guid? UserId { get; set; }

        public string? Title { get; set; }

        public Guid? PlateId { get; set; }

        //默认查询非置顶
        public bool? IsTop { get; set; } 


        //查询方式
        public QueryDiscussTypeEnum Type { get; set; } = QueryDiscussTypeEnum.New;
    }
}
