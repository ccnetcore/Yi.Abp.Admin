using Yi.Framework.Bbs.Application.Contracts.Dtos.Plate;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Plate服务抽象
    /// </summary>
    public interface IPlateService : IYiCrudAppService<PlateGetOutputDto, PlateGetListOutputDto, Guid, PlateGetListInputVo, PlateCreateInputVo, PlateUpdateInputVo>
    {

    }
}
