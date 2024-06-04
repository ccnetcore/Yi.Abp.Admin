using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Domain.Entities
{
    /// <summary>
    /// 角色表
    /// </summary>
    [SugarTable("Role")]
    public class RoleAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject, IOrderNum, IState
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 最后修改者
        /// </summary>
        public Guid? LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastModificationTime { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;


        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色编码 
        ///</summary>
        [SugarColumn(ColumnName = "RoleCode")]
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// 描述 
        ///</summary>
        [SugarColumn(ColumnName = "Remark")]
        public string? Remark { get; set; }
        /// <summary>
        /// 角色数据范围 
        ///</summary>
        [SugarColumn(ColumnName = "DataScope")]
        public DataScopeEnum DataScope { get; set; } = DataScopeEnum.ALL;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;


        [Navigate(typeof(RoleMenuEntity), nameof(RoleMenuEntity.RoleId), nameof(RoleMenuEntity.MenuId))]
        public List<MenuAggregateRoot>? Menus { get; set; }

        [Navigate(typeof(RoleDeptEntity), nameof(RoleDeptEntity.RoleId), nameof(RoleDeptEntity.DeptId))]
        public List<DeptAggregateRoot>? Depts { get; set; }
    }
}
