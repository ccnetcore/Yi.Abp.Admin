using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// User服务抽象
    /// </summary>
    public interface IUserService : IYiCrudAppService<UserGetOutputDto, UserGetListOutputDto, Guid, UserGetListInputVo, UserCreateInputVo, UserUpdateInputVo>
    {
    }
}
