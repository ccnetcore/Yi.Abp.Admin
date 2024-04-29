using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IAccountService : IApplicationService
    {
        Task<UserRoleMenuDto> GetAsync();
        Task<CaptchaImageDto> GetCaptchaImageAsync();
        Task<object> PostLoginAsync(LoginInputVo input);
        Task PostRegisterAsync(RegisterDto input);
        Task<bool> RestPasswordAsync(Guid userId, RestPasswordDto input);
    }
}
