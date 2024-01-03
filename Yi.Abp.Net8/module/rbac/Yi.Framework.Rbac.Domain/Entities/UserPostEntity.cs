using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Rbac.Domain.Entities;
/// <summary>
/// 用户岗位表
///</summary>
[SugarTable("UserPost")]
public partial class UserPostEntity : Entity<Guid>
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }
    /// <summary>
    /// 用户id
    /// </summary>
    [SugarColumn(ColumnName = "UserId")]
    public Guid UserId { get; set; }
    /// <summary>
    /// 岗位id 
    ///</summary>
    [SugarColumn(ColumnName = "PostId")]
    public Guid PostId { get; set; }

}
