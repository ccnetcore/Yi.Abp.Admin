using System.Text.RegularExpressions;
using Lazy.Captcha.Core;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Guids;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Managers;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.Domain.Shared.Caches;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Shared.Etos;
using Yi.Framework.Rbac.Domain.Shared.Options;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services
{
    public class AccountService : ApplicationService, IAccountService
    {
        protected ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();
        private IDistributedCache<CaptchaPhoneCacheItem, CaptchaPhoneCacheKey> _phoneCache;
        private readonly ICaptcha _captcha;
        private readonly IGuidGenerator _guidGenerator;
        private readonly RbacOptions _rbacOptions;
        private readonly IAliyunManger _aliyunManger;
        private IDistributedCache<UserInfoCacheItem, UserInfoCacheKey> _userCache;
        private UserManager _userManager;
        private IHttpContextAccessor _httpContextAccessor;

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
            UserManager userManager, IHttpContextAccessor httpContextAccessor)
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
            _httpContextAccessor = httpContextAccessor;
        }


        private IUserRepository _userRepository;
        private ICurrentUser _currentUser;
        private IAccountManager _accountManager;
        private ISqlSugarRepository<MenuAggregateRoot> _menuRepository;

        /// <summary>
        /// 校验图片登录验证码,无需和账号绑定
        /// </summary>
        [RemoteService(isEnabled:false)]
        public void ValidationImageCaptcha(string? uuid,string? code )
        {
            if (_rbacOptions.EnableCaptcha)
            {
                //登录不想要验证码 ，可不校验
                if (!_captcha.Validate(uuid, code))
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
        public async Task<LoginOutputDto> PostLoginAsync(LoginInputVo input)
        {
            if (string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.UserName))
            {
                throw new UserFriendlyException("请输入合理数据！");
            }

            //校验验证码
            ValidationImageCaptcha(input.Uuid,input.Code);

            UserAggregateRoot user = new();
            //校验
            await _accountManager.LoginValidationAsync(input.UserName, input.Password, x => user = x);

            return await PostLoginAsync(user.Id);
        }


        /// <summary>
        /// 提供其他服务使用，根据用户id，直接返回token
        /// </summary>
        /// <returns></returns>
        [RemoteService(isEnabled: false)]
        public async Task<LoginOutputDto> PostLoginAsync(Guid userId)
        {
            var userInfo = new UserRoleMenuDto();
            //获取token
            var accessToken = await _accountManager.GetTokenByUserIdAsync(userId, (info) => userInfo = info);
            var refreshToken = _accountManager.CreateRefreshToken(userId);

            //这里抛出一个登录的事件,也可以在全部流程走完，在应用层组装
            if (_httpContextAccessor.HttpContext is not null)
            {
                var loginEntity = new LoginLogAggregateRoot().GetInfoByHttpContext(_httpContextAccessor.HttpContext);
                var loginEto = loginEntity.Adapt<LoginEventArgs>();
                loginEto.UserName = userInfo.User.UserName;
                loginEto.UserId = userInfo.User.Id;
                await LocalEventBus.PublishAsync(loginEto);
            }

            return new LoginOutputDto { Token = accessToken, RefreshToken = refreshToken };
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
            var enableCaptcha = _rbacOptions.EnableCaptcha;
            return new CaptchaImageDto { Img = captcha.Bytes, Uuid = uuid, IsEnableCaptcha = enableCaptcha };
        }

        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="str_handset"></param>
        private async Task ValidationPhone(string phone)
        {
            var res = Regex.IsMatch(phone, @"^\d{11}$");
            if (res == false)
            {
                throw new UserFriendlyException("手机号码格式错误！请检查");
            }
        }


        /// <summary>
        /// 手机验证码-注册
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("account/captcha-phone")]
        [AllowAnonymous]
        public async Task<object> PostCaptchaPhoneForRegisterAsync(PhoneCaptchaImageDto input)
        {
            return await PostCaptchaPhoneAsync(ValidationPhoneTypeEnum.Register, input);
        }

        /// <summary>
        /// 手机验证码-找回密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("account/captcha-phone/repassword")]
        public async Task<object> PostCaptchaPhoneForRetrievePasswordAsync(PhoneCaptchaImageDto input)
        {
            return await PostCaptchaPhoneAsync(ValidationPhoneTypeEnum.RetrievePassword, input);
        }
        
        /// <summary>
        /// 手机验证码-绑定
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("account/captcha-phone/bind")]
        [AllowAnonymous]
        public async Task<object> PostCaptchaPhoneForBindAsync(PhoneCaptchaImageDto input)
        {
            return await PostCaptchaPhoneAsync(ValidationPhoneTypeEnum.Bind, input);
        }
        
        /// <summary>
        /// 手机验证码-需通过图形验证码
        /// </summary>
        /// <returns></returns>
        [RemoteService(isEnabled:false)]
        private async Task<object> PostCaptchaPhoneAsync(ValidationPhoneTypeEnum validationPhoneType,
            PhoneCaptchaImageDto input)
        {
            //验证uuid 和 验证码
            ValidationImageCaptcha(input.Uuid,input.Code);
            
            await ValidationPhone(input.Phone);
            
            if (validationPhoneType == ValidationPhoneTypeEnum.Register &&
                await _userRepository.IsAnyAsync(x => x.Phone.ToString() == input.Phone))
            {
                throw new UserFriendlyException("该手机号已被注册！");
            }

            var value = await _phoneCache.GetAsync(new CaptchaPhoneCacheKey(validationPhoneType, input.Phone));

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

            await _phoneCache.SetAsync(new CaptchaPhoneCacheKey(validationPhoneType, input.Phone),
                new CaptchaPhoneCacheItem(code),
                new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(10) });
            return new
            {
                Uuid = uuid
            };
        }

        /// <summary>
        /// 校验电话验证码，需要与电话号码绑定
        /// </summary>
        public async Task ValidationPhoneCaptchaAsync(ValidationPhoneTypeEnum validationPhoneType, long phone,
            string code)
        {
            var item = await _phoneCache.GetAsync(new CaptchaPhoneCacheKey(validationPhoneType, phone.ToString()));
            if (item is not null && item.Code.Equals($"{code}"))
            {
                //成功，需要清空
                await _phoneCache.RemoveAsync(new CaptchaPhoneCacheKey(validationPhoneType, code.ToString()));
                return;
            }

            throw new UserFriendlyException("验证码错误");
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="input"></param>
        [AllowAnonymous]
        [UnitOfWork]
        public async Task<string> PostRetrievePasswordAsync(RetrievePasswordDto input)
        {
            //校验验证码，根据电话号码获取 value，比对验证码已经uuid
            await ValidationPhoneCaptchaAsync(ValidationPhoneTypeEnum.RetrievePassword, input.Phone, input.Code);

            var entity = await _userRepository.GetFirstAsync(x => x.Phone == input.Phone);
            if (entity is null)
            {
                throw new UserFriendlyException("该手机号码未注册");
            }

            await _accountManager.RestPasswordAsync(entity.Id, input.Password);

            return entity.UserName;
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

            if (input.Phone is null)
            {
                throw new UserFriendlyException("手机号不能为空");
            }
            //临时账号
            if (input.UserName.StartsWith("ls_"))
            {
                throw new UserFriendlyException("注册账号不能以ls_字符开头");
            }
            
            if (_rbacOptions.EnableCaptcha)
            {
                //校验验证码，根据电话号码获取 value，比对验证码已经uuid
                await ValidationPhoneCaptchaAsync(ValidationPhoneTypeEnum.Register, input.Phone.Value, input.Code);
            }

            //注册领域逻辑
            await _accountManager.RegisterAsync(input.UserName, input.Password, input.Phone, input.Nick);
        }

        /// <summary>
        /// 临时注册
        /// 不需要验证，为了给第三方使用，例如微信小程序，后续可通过绑定操作，进行账号合并
        /// </summary>
        /// <param name="input"></param>
        [RemoteService(isEnabled:false)]
        public async Task PostTempRegisterAsync(RegisterDto input)
        {
            //注册领域逻辑
            await _accountManager.RegisterAsync(input.UserName, input.Password, input.Phone, input.Nick);
        }

        /// <summary>
        /// 查询已登录的账户信息
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

        [RemoteService(isEnabled: false)]
        public async Task<UserRoleMenuDto?> GetAsync(string? userName, long? phone)
        {
            var user = await _userRepository._DbQueryable
                .WhereIF(userName is not null, x => x.UserName == userName)
                .WhereIF(phone is not null, x => x.Phone == phone)
                .Where(x => x.State == true)
                .FirstAsync();

            //该用户不存在
            if (user is null)
            {
                return null;
            }

            //注意用户名大小写数据库不敏感问题
            if (userName is not null && !user.UserName.Equals(userName))
            {
                throw new UserFriendlyException($"该用户名不存在或已禁用-{userName}");
            }

            var output = await _userManager.GetInfoAsync(user.Id);
            return output;
        }

        /// <summary>
        /// 获取当前登录用户的前端路由
        /// 支持ruoyi/pure
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("account/Vue3Router/{routerType?}")]
        public async Task<object> GetVue3Router([FromRoute] string? routerType)
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

            object output = null;
            if (routerType is null || routerType == "ruoyi")
            {
                //将后端菜单转换成前端路由，组件级别需要过滤
                output =
                    ObjectMapper.Map<List<MenuDto>, List<MenuAggregateRoot>>(menus.Where(x=>x.MenuSource==MenuSourceEnum.Ruoyi).ToList()).Vue3RuoYiRouterBuild();
            }
            else if (routerType == "pure")
            {
                //将后端菜单转换成前端路由，组件级别需要过滤
                output =
                    ObjectMapper.Map<List<MenuDto>, List<MenuAggregateRoot>>(menus.Where(x=>x.MenuSource==MenuSourceEnum.Pure).ToList()).Vue3PureRouterBuild();
            }

            return output;
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

            await _accountManager.UpdatePasswordAsync(_currentUser.Id ?? Guid.Empty, input.NewPassword,
                input.OldPassword);
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
            Guid userId = input.UserId == null ? _currentUser.GetId() : input.UserId.Value;

            var entity = await _userRepository.GetByIdAsync(userId);

            if (entity.Icon == input.Icon)
            {
                return false;
            }

            entity.Icon = input.Icon;
            await _userRepository.UpdateAsync(entity);

            //发布更新头像任务事件
            await this.LocalEventBus.PublishAsync(
                new AssignmentEventArgs(AssignmentRequirementTypeEnum.UpdateIcon, userId), false);
            return true;
        }
    }
}