using System.Text.RegularExpressions;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Managers;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Shared.Caches;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Options;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services
{

    public class AccountService : ApplicationService, IAccountService
    {
        private IDistributedCache<CaptchaPhoneCacheItem, CaptchaPhoneCacheKey> _phoneCache;
        private readonly ICaptcha _captcha;
        private readonly IGuidGenerator _guidGenerator;
        private readonly RbacOptions _rbacOptions;
        private readonly IAliyunManger _aliyunManger;
        private IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> _userCache;
        private UserManager _userManager;
        public AccountService(IUserRepository userRepository,
            ICurrentUser currentUser,
            IAccountManager accountManager,
            ISqlSugarRepository<MenuAggregateRoot> menuRepository,
            IDistributedCache<CaptchaPhoneCacheItem, CaptchaPhoneCacheKey> phoneCache,
            IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> userCache,
            ICaptcha captcha,
            IGuidGenerator guidGenerator,
            IOptions<RbacOptions> options,
            IAliyunManger aliyunManger,
            UserManager userManager)
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _accountManager = accountManager;
            _menuRepository = menuRepository;
            _phoneCache = phoneCache;
            _captcha = captcha;
            _guidGenerator = guidGenerator;
            _rbacOptions = options.Value;
            _aliyunManger = aliyunManger;
            _userCache = userCache;
            _userManager = userManager;
        }


        private IUserRepository _userRepository;
        private ICurrentUser _currentUser;
        private IAccountManager _accountManager;
        private ISqlSugarRepository<MenuAggregateRoot> _menuRepository;
        /// <summary>
        /// 校验图片登录验证码,无需和账号绑定
        /// </summary>
        [AllowAnonymous]
        private void ValidationImageCaptcha(LoginInputVo input)
        {
            if (_rbacOptions.EnableCaptcha)
            {
                //登录不想要验证码 ，可不校验
                if (!_captcha.Validate(input.Uuid, input.Code))
                {
                    throw new UserFriendlyException("验证码错误");
                }
            }
        }



        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<object> PostLoginAsync(LoginInputVo input)
        {
            if (string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.UserName))
            {
                throw new UserFriendlyException("请输入合理数据！");
            }

            //校验验证码
            ValidationImageCaptcha(input);

            UserAggregateRoot user = new();
            //校验
            await _accountManager.LoginValidationAsync(input.UserName, input.Password, x => user = x);

            //获取token
            var accessToken = await _accountManager.GetTokenByUserIdAsync(user.Id);
            var refreshToken = _accountManager.CreateRefreshToken(user.Id);

            return new { Token = accessToken, RefreshToken = refreshToken };
        }


        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = TokenTypeConst.Refresh)]
        public async Task<object> PostRefreshAsync([FromQuery] string refresh_token)
        {
            var userId = CurrentUser.Id.Value;
            var accessToken = await _accountManager.GetTokenByUserIdAsync(userId);
            var refreshToken = _accountManager.CreateRefreshToken(userId);
            return new { Token = accessToken, RefreshToken = refreshToken };
        }

        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>

        [AllowAnonymous]
        public async Task<CaptchaImageDto> GetCaptchaImageAsync()
        {
            var uuid = _guidGenerator.Create();
            var captcha = _captcha.Generate(uuid.ToString());
            return new CaptchaImageDto { Img = captcha.Bytes, Uuid = uuid };
        }

        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="str_handset"></param>
        private async Task ValidationPhone(string str_handset)
        {
            var res = Regex.IsMatch(str_handset, @"^\d{11}$");
            if (res == false)
            {
                throw new UserFriendlyException("手机号码格式错误！请检查");
            }
            if (await _userRepository.IsAnyAsync(x => x.Phone.ToString() == str_handset))
            {
                throw new UserFriendlyException("该手机号已被注册！");

            }

        }


        /// <summary>
        /// 注册 手机验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<object> PostCaptchaPhone(PhoneCaptchaImageDto input)
        {
            await ValidationPhone(input.Phone);
            var value = await _phoneCache.GetAsync(new CaptchaPhoneCacheKey(input.Phone));

            //防止暴刷
            if (value is not null)
            {
                throw new UserFriendlyException($"{input.Phone}已发送过验证码，10分钟后可重试");
            }
            //生成一个4位数的验证码
            //发送短信，同时生成uuid
            ////key： 电话号码  value:验证码+uuid  
            var code = Guid.NewGuid().ToString().Substring(0, 4);
            var uuid = Guid.NewGuid();
            await _aliyunManger.SendSmsAsync(input.Phone, code);

            await _phoneCache.SetAsync(new CaptchaPhoneCacheKey(input.Phone), new CaptchaPhoneCacheItem(code), new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
            return new
            {
                Uuid = uuid
            };
        }

        /// <summary>
        /// 校验电话验证码，需要与电话号码绑定
        /// </summary>
        private async Task ValidationPhoneCaptchaAsync(RegisterDto input)
        {
            var item = await _phoneCache.GetAsync(new CaptchaPhoneCacheKey(input.Phone.ToString()));
            if (item is not null && item.Code.Equals($"{input.Code}"))
            {
                //成功，需要清空
                await _phoneCache.RemoveAsync(new CaptchaPhoneCacheKey(input.Phone.ToString()));
                return;
            }
            throw new UserFriendlyException("验证码错误");
        }

        /// <summary>
        /// 注册，需要验证码通过
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [UnitOfWork]
        public async Task PostRegisterAsync(RegisterDto input)
        {
            if (_rbacOptions.EnableRegister == false)
            {
                throw new UserFriendlyException("该系统暂未开放注册功能");
            }
            if (_rbacOptions.EnableCaptcha)
            {
                //校验验证码，根据电话号码获取 value，比对验证码已经uuid
                await ValidationPhoneCaptchaAsync(input);
            }
            //注册领域逻辑
            await _accountManager.RegisterAsync(input.UserName, input.Password, input.Phone);
        }


        /// <summary>
        /// 查询已登录的账户信息，已缓存
        /// </summary>
        /// <returns></returns>
        [Route("account")]
        [Authorize]

        public async Task<UserRoleMenuDto> GetAsync()
        {
            //通过鉴权jwt获取到用户的id
            var userId = _currentUser.Id;
            if (userId is null)
            {
                throw new UserFriendlyException("用户未登录");
            }
            //此处优先从缓存中获取
            var output = await _userManager.GetInfoAsync(userId.Value);
            return output;
        }


        /// <summary>
        /// 获取当前登录用户的前端路由
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("account/Vue3Router")]
        public async Task<List<Vue3RouterDto>> GetVue3Router()
        {
            var userId = _currentUser.Id;
            if (_currentUser.Id is null)
            {
                throw new AbpAuthorizationException("用户未登录");

            }
            var data = await _userManager.GetInfoAsync(userId!.Value);
            var menus = data.Menus.ToList();

            //为超级管理员直接给全部路由
            if (UserConst.Admin.Equals(data.User.UserName))
            {
                menus = ObjectMapper.Map<List<MenuAggregateRoot>, List<MenuDto>>(await _menuRepository.GetListAsync());
            }
            //将后端菜单转换成前端路由，组件级别需要过滤
            List<Vue3RouterDto> routers = ObjectMapper.Map<List<MenuDto>, List<MenuAggregateRoot>>(menus).Vue3RouterBuild();
            return routers;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PostLogout()
        {
            //通过鉴权jwt获取到用户的id
            var userId = _currentUser.Id;
            if (userId is null)
            {
                return false;
                // throw new UserFriendlyException("用户已退出");
            }
            await _userCache.RemoveAsync(new UserInfoCacheKey(userId.Value));
            //Jwt去中心化登出，只需用记录日志即可
            return true;
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePasswordAsync(UpdatePasswordDto input)
        {
            if (input.OldPassword.Equals(input.NewPassword))
            {
                throw new UserFriendlyException("无效更新！输入的数据，新密码不能与老密码相同");
            }
            if (_currentUser.Id is null)
            {
                throw new UserFriendlyException("用户未登录");
            }
            await _accountManager.UpdatePasswordAsync(_currentUser.Id ?? Guid.Empty, input.NewPassword, input.OldPassword);
            return true;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<bool> RestPasswordAsync(Guid userId, RestPasswordDto input)
        {
            if (string.IsNullOrEmpty(input.Password))
            {
                throw new UserFriendlyException("重置密码不能为空！");
            }
            await _accountManager.RestPasswordAsync(userId, input.Password);
            return true;
        }

        /// <summary>
        ///  更新头像
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> UpdateIconAsync(UpdateIconDto input)
        {
            var entity = await _userRepository.GetByIdAsync(_currentUser.Id);
            entity.Icon = input.Icon;
            await _userRepository.UpdateAsync(entity);

            return true;
        }
    }
}
