using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// User服务抽象
    /// </summary>
    public interface IUserService : IYiCrudAppService<UserGetOutputDto, UserGetListOutputDto, Guid, UserGetListInputVo, UserCreateInputVo, UserUpdateInputVo>
    {
        /// <summary>
        /// 获取指定部门及其所有子部门下的用户列表
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns>用户列表</returns>
        Task<List<UserGetListOutputDto>> GetUsersByDeptAsync(Guid deptId);
    }
}
