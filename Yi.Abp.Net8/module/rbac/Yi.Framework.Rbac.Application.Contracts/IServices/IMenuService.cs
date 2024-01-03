using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Menu;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// Menu服务抽象
    /// </summary>
    public interface IMenuService : IYiCrudAppService<MenuGetOutputDto, MenuGetListOutputDto, Guid, MenuGetListInputVo, MenuCreateInputVo, MenuUpdateInputVo>
    {

    }
}
