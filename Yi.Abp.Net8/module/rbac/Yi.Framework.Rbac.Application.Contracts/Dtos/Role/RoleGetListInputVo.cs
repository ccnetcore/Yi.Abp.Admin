using Yi.Framework.Ddd;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Role
{
    public class RoleGetListInputVo : PagedAllResultRequestDto
    {
        public string? RoleName { get; set; }
        public string? RoleCode { get; set; }
        public bool? State { get; set; }


    }
}
