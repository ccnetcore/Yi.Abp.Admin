using Mapster;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.User
{
    public class UserUpdateInputVo
    {
        public string? Name { get; set; }
        public int? Age { get; set; }
        public string? UserName { get; set; }

        /// <summary>
        /// 该成员需忽略
        /// </summary>
        [AdaptIgnore]
        public string? Password { get; set; }
        public string? Icon { get; set; }
        public string? Nick { get; set; }
        public string? Email { get; set; }
        public string? Ip { get; set; }
        public string? Address { get; set; }
        public long? Phone { get; set; }
        public string? Introduction { get; set; }
        public string? Remark { get; set; }
        public SexEnum? Sex { get; set; }
        public Guid? DeptId { get; set; }
        public List<Guid>? PostIds { get; set; }

        public List<Guid>? RoleIds { get; set; }
        public bool? State { get; set; }
    }
}
