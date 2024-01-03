using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Banner
{
    public class BannerGetListOutputDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Color { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
