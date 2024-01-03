using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss
{
    public class DiscussGetListInputVo : PagedAndSortedResultRequestDto
    {
        public string? Title { get; set; }

        public Guid? PlateId { get; set; }

        //默认查询非置顶
        public bool IsTop { get; set; } = false;


        //查询方式
        public QueryDiscussTypeEnum Type { get; set; } = QueryDiscussTypeEnum.New;
    }
}
