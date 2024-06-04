using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Account;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Application.Services.System;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Test;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Test.System
{
    public class Account_Test : YiTestWebBase
    {

        private IAccountService _accountService;
        private ISqlSugarRepository<UserAggregateRoot> _userRepository;
        public Account_Test()
        {
            _accountService = GetRequiredService<IAccountService>();
            _userRepository = GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
        }

        /// <summary>
        /// 注册
        /// </summary>
        [Fact]
        public async Task Register_Test()
        {
            await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "RegisterTest", Password = "123456", Phone = 15945645645 });
            var user = await _userRepository._DbQueryable.Where(user => user.UserName == "RegisterTest").FirstAsync();
            user.ShouldNotBeNull();
            user.JudgePassword("123456").ShouldBeTrue();
        }

        /// <summary>
        /// 用户名重复注册
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Register_UserNameRepeat_Error_Test()
        {
            try
            {
                await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "RegisterUserNameRepeatErrorTest", Password = "123456", Phone = 15945645641 });
                await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "RegisterUserNameRepeatErrorTest", Password = "123456", Phone = 15945645642 });
            }
            catch (UserFriendlyException ex)
            {
                ex.Message.ShouldBe(UserConst.User_Exist);
            }
        }

        /// <summary>
        /// 电话号码重复注册
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Register_PhoneRepeat_Error_Test()
        {
            try
            {
                await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "RegisterPhoneRepeatErrorTest1", Password = "123456", Phone = 15945645633 });
                await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "RegisterPhoneRepeatErrorTest2", Password = "123456", Phone = 15945645633 });
            }
            catch (UserFriendlyException ex)
            {
                ex.Message.ShouldBe(UserConst.Phone_Repeat);
            }
        }


        /// <summary>
        /// 登录测试
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Login_Test()
        {
            await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "LoginTest", Password = "123456", Phone = 13845645645 });
            var result = await _accountService.PostLoginAsync(new LoginInputVo { UserName = "LoginTest", Password = "123456" });

            result.GetType().GetProperty("Token").GetValue(result, null).ToString().ShouldNotBeNull();
            result.GetType().GetProperty("RefreshToken").GetValue(result, null).ToString().ShouldNotBeNull();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Reset_Passworld_Test()
        {
            await _accountService.PostRegisterAsync(new RegisterDto() { UserName = "ResetPassworldTest", Password = "123456", Phone = 15945645555 });
            var user = await _userRepository._DbQueryable.Where(user => user.UserName == "ResetPassworldTest").FirstAsync();
            await _accountService.RestPasswordAsync(user.Id, new RestPasswordDto { Password = "654321abc" });
            var result = await _accountService.PostLoginAsync(new LoginInputVo { UserName = "ResetPassworldTest", Password = "654321abc" });

        }
    }
}
