using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.MyType
{
    public class DiscussLableGetListInputVo : PagedAndSortedResultRequestDto
    {
        public string? Name { get; set; }
    }
}
