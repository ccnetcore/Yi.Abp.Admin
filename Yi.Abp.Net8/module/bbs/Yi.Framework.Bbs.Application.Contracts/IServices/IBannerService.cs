using Yi.Framework.Bbs.Application.Contracts.Dtos.Banner;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Banner抽象
    /// </summary>
    public interface IBannerService : IYiCrudAppService<BannerGetOutputDto, BannerGetListOutputDto, Guid, BannerGetListInputVo, BannerCreateInputVo, BannerUpdateInputVo>
    {

    }
}
