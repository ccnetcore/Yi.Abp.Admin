using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IAccountService
    {
        Task<UserRoleMenuDto> Get();
        Task<object> PostLoginAsync(LoginInputVo input);
    }
}
