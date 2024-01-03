using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Banner
{
    public class BannerGetListInputVo : PagedAndSortedResultRequestDto
    {
        public string? Name { get; set; }
    }
}
