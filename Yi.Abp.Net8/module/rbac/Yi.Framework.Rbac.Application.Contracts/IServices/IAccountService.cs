using Volo.Abp.Application.Services;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IAccountService : IApplicationService
    {
        Task<UserRoleMenuDto> GetAsync();
        Task<CaptchaImageDto> GetCaptchaImageAsync();
        Task<LoginOutputDto> PostLoginAsync(LoginInputVo input);
        Task PostRegisterAsync(RegisterDto input);
        Task<bool> RestPasswordAsync(Guid userId, RestPasswordDto input);

        /// <summary>
        /// 提供其他服务使用，根据用户id，直接返回token
        /// </summary>
        /// <returns></returns>
        Task<LoginOutputDto> PostLoginAsync(Guid userId);

        /// <summary>
        /// 根据信息查询用户，可能为空，代表该用户不存在或禁用
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<UserRoleMenuDto?> GetAsync(string? userName,long? phone);

        /// <summary>
        /// 校验电话验证码，需要与电话号码绑定
        /// </summary>
        Task ValidationPhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, long phone,
            string code);

        /// <summary>
        /// 临时注册
        /// 不需要验证，为了给第三方使用，例如微信小程序，后续可通过绑定操作，进行账号合并
        /// </summary>
        /// <param name="input"></param>
        Task PostTempRegisterAsync(RegisterDto input);
    }
}
