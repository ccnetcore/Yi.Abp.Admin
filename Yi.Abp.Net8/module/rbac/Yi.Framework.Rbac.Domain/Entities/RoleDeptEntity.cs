using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Entities;

/// <summary>
/// 角色部门关系表
///</summary>
[SugarTable("RoleDept")]
public  class RoleDeptEntity : Entity<Guid>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }

    /// <summary>
    /// 角色id 
    ///</summary>
    [SugarColumn(ColumnName = "RoleId")]
    public Guid RoleId { get; set; }
    /// <summary>
    /// 部门id 
    ///</summary>
    [SugarColumn(ColumnName = "DeptId")]
    public Guid DeptId { get; set; }


}
