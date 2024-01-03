using Yi.Framework.Ddd;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.LoginLog
{
    public class LoginLogGetListInputVo : PagedAllResultRequestDto
    {
        public string? LoginUser { get; set; }

        public string? LoginIp { get; set; }
    }
}
