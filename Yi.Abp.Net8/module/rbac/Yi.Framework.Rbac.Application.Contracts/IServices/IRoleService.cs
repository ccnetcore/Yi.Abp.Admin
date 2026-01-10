using Microsoft.AspNetCore.Mvc;
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
        /// <summary>
        /// 获取角色菜单树
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色菜单树数据，包含已选中的菜单ID和菜单树结构</returns>
        Task<ActionResult> GetMenuTreeAsync(Guid roleId);

        /// <summary>
        /// 获取角色部门树
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色部门树数据，包含已选中的部门ID和部门树结构</returns>
        Task<ActionResult> GetDeptTreeAsync(Guid roleId);
    }
}
