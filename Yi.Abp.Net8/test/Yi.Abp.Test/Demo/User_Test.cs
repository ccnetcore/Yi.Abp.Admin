using Shouldly;
using Xunit;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Shared.Consts;

namespace Yi.Abp.Test.Demo
{
    public class User_Test : YiAbpTestBase
    {
        [Fact]
        public async Task Get_User_List_Test()
        {
            var service = GetRequiredService<IUserService>();
            var user = await service.GetListAsync(new Framework.Rbac.Application.Contracts.Dtos.User.UserGetListInputVo { UserName = UserConst.Admin });
            user.ShouldNotBeNull();
        }
    }
}
