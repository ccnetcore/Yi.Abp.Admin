using Yi.Framework.Ddd;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.User
{
    public class UserGetListInputVo : PagedAllResultRequestDto
    {
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public long? Phone { get; set; }

        public bool? State { get; set; }

        public Guid? DeptId { get; set; }

        public string? Ids { get; set; }
    }
}
