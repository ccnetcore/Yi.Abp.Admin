using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Tiw.V20190919.Models;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Enums;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Test.System
{
    public class User_Test : YiTestBase
    {
        private IUserService _userService;
        private ISqlSugarRepository<UserAggregateRoot> _repository;
        public User_Test()
        {
            _userService = ServiceProvider.GetRequiredService<IUserService>();
            _repository = ServiceProvider.GetRequiredService<ISqlSugarRepository<UserAggregateRoot>>();
        }

        /// <summary>
        /// 查询用户
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_User_Test()
        {
            var user = await _userService.GetListAsync(new UserGetListInputVo { UserName = UserConst.Admin });
            user.ShouldNotBeNull();
        }


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Create_User_Test()
        {
            await _userService.CreateAsync(new UserCreateInputVo { UserName = "CreateUserTest", Password = "654321" });
            var user = await _userService.GetListAsync(new UserGetListInputVo { UserName = "CreateUserTest" });
            user.ShouldNotBeNull();
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Update_User_Test()
        {
            var createdUser = await _userService.CreateAsync(new UserCreateInputVo { Nick = "nickTest", Sex = SexEnum.Woman, UserName = "UpdateUserTest", Password = "654321" });
            await _userService.UpdateAsync(createdUser.Id, new UserUpdateInputVo { Nick = "nickTest2", Sex = SexEnum.Woman, UserName = "UpdateUserTest", Password = "123456888abc" });
            var user = await _repository._DbQueryable.Where(user => user.UserName == "UpdateUserTest").FirstAsync();
            user.ShouldNotBeNull();
            user.Nick.ShouldBe("nickTest2");
            user.Sex.ShouldBe(SexEnum.Woman);
            user.JudgePassword("123456888abc");
        }


        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Delete_User_Test()
        {
            var createdUser = await _userService.CreateAsync(new UserCreateInputVo { UserName = "DeleteUserTest", Password = "123456" });

            var user1 = await _repository._DbQueryable.Where(user => user.UserName == "DeleteUserTest").FirstAsync();
            user1.ShouldNotBeNull();

            await _userService.DeleteAsync(new List<Guid> { createdUser.Id });
            var user2 = await _repository._DbQueryable.Where(user => user.UserName == "DeleteUserTest").FirstAsync();
            user2.ShouldBeNull();
        }
    }
}
