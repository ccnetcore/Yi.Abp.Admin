using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Role
{
    public class UpdateDataScpoceInput
    {
        public Guid RoleId { get; set; }

        public List<Guid>? DeptIds { get; set; }

        public DataScopeEnum DataScope { get; set; }
    }
}
