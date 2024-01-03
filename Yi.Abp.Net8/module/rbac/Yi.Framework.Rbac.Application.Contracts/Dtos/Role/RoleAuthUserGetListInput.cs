using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Role
{
    public class RoleAuthUserGetListInput : PagedAndSortedResultRequestDto
    {
        public string? UserName { get; set; }

        public long? Phone { get; set; }
    }
}
