using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Role;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// Role服务抽象
    /// </summary>
    public interface IRoleService : IYiCrudAppService<RoleGetOutputDto, RoleGetListOutputDto, Guid, RoleGetListInputVo, RoleCreateInputVo, RoleUpdateInputVo>
    {

    }
}
