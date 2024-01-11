using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Managers;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services.Authentication
{
    /// <summary>
    /// 第三方授权服务
    /// </summary>
    public class AuthService : YiCrudAppService<AuthAggregateRoot, AuthOutputDto, Guid, AuthGetListInput>
    {
        private HttpContext HttpContext { get; set; }
        private ILogger<AuthService> _logger;
        private ISqlSugarRepository<AuthAggregateRoot, Guid> _repository;
        private IAccountManager _accountManager;
        public AuthService(IAccountManager accountManager, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger, ISqlSugarRepository<AuthAggregateRoot, Guid> repository) : base(repository)
        {
            _logger = logger;
            HttpContext = httpContextAccessor.HttpContext ?? throw new ApplicationException("未注册Http");
            _repository = repository;
            _accountManager = accountManager;
        }

        /// <summary>
        /// 第三方oauth登录
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="code">code是为了swagger更好的处理和显示</param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpGet("auth/oauth/login/{scheme}")]
        public async Task<object> AuthOauthLoginAsync([FromRoute] string scheme, [FromQuery] string code)
        {
            (var openId, var _) = await GetOpenIdAndNameAsync(scheme);
            var authEntity = await _repository.GetAsync(x => x.OpenId == openId && x.AuthType == scheme);

            if (authEntity is null)
            {
                throw new UserFriendlyException("第三方登录失败，请先注册后，在个人中心进行绑定该第三方后使用");
            }
            var accessToken = await _accountManager.GetTokenByUserIdAsync(authEntity.UserId);
            return new { token= accessToken };
        }

        /// <summary>
        /// 第三方oauth绑定
        /// </summary>
        /// <param name="scheme"></param>
        /// <param name="code">code是为了swagger更好的处理和显示</param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException"></exception>
        [HttpPost("auth/oauth/bind/{scheme}")]
        [Authorize]
        public async Task AuthOauthBindAsync([FromRoute] string scheme, [FromQuery] string code)
        {
            (var openId, var name) = await GetOpenIdAndNameAsync(scheme);
            var userId = CurrentUser.Id;
            var authEntityAny = await _repository.IsAnyAsync(x => x.OpenId == openId && x.AuthType == scheme);
            if (authEntityAny)
            {
                throw new UserFriendlyException("绑定失败，该第三方账号已被注册");
            }
            var authAggregateRoot = new AuthAggregateRoot(scheme, userId ?? Guid.Empty, openId, name);

            await _repository.InsertAsync(authAggregateRoot);
        }


        private async Task<(string, string)> GetOpenIdAndNameAsync(string scheme)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(scheme);
            if (!authenticateResult.Succeeded)
            {
                throw new UserFriendlyException(authenticateResult.Failure.Message);
            }
            var openidClaim = authenticateResult.Principal.Claims.Where(x => x.Type == "urn:openid").FirstOrDefault();
            var nameClaim = authenticateResult.Principal.Claims.Where(x => x.Type == "urn:name").FirstOrDefault();
            return (openidClaim.Value, nameClaim.Value);
        }


        /// <summary>
        /// 获取当前账户的授权信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IReadOnlyList<AuthOutputDto>> GetListAccountAsync(AuthGetListInput input)
        {
            input.UserId = CurrentUser.Id;
            input.MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount;
            input.SkipCount = 1;
            return (await GetListAsync(input)).Items;
        }


        public override async Task<PagedResultDto<AuthOutputDto>> GetListAsync(AuthGetListInput input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable.WhereIF(input.UserId is not null, x => x.UserId == input.UserId)
                          .WhereIF(!string.IsNullOrEmpty(input.AuthType), x => x.AuthType == input.AuthType)
                          .WhereIF(!string.IsNullOrEmpty(input.OpenId), x => x.OpenId == input.OpenId)
                          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<AuthOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }

        [RemoteService(IsEnabled = false)]
        public override Task<AuthOutputDto> UpdateAsync(Guid id, AuthOutputDto input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 删除第三方授权
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RemoteService(IsEnabled = true)]
        public override Task DeleteAsync(IEnumerable<Guid> id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(IsEnabled = false)]
        public override Task<AuthOutputDto> CreateAsync(AuthOutputDto input)
        {
            throw new NotImplementedException();
        }
    }
}
