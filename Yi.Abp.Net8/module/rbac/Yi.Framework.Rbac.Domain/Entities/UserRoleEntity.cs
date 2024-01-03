using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Entities
{
    /// <summary>
    /// 用户角色关系表
    ///</summary>
    [SugarTable("UserRole")]
    public partial class UserRoleEntity : Entity<Guid>
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }
    }
}
