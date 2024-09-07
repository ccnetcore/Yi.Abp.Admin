using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Bbs.Domain.Entities.Assignment;

/// <summary>
/// 任务定义表
/// </summary>
[SugarTable("AssignmentDefine")]

public class AssignmentDefineAggregateRoot: AggregateRoot<Guid>, IHasCreationTime,IOrderNum
{
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public override Guid Id { get; protected set; }
    
    /// <summary>
    /// 任务名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 备注
    /// </summary>
    public string Remarks { get; set; }
    
    /// <summary>
    /// 任务类型
    /// </summary>
    public AssignmentTypeEnum AssignmentType{ get; set; }

    /// <summary>
    /// 任务需求类型
    /// </summary>
    public AssignmentRequirementTypeEnum AssignmentRequirementType{ get; set; }
    
    /// <summary>
    /// 总共步骤数
    /// </summary>
    public int TotalStepNumber { get; set; }
    
    /// <summary>
    /// 前置任务id
    /// </summary>
    public Guid? PreAssignmentId { get; set; }

    /// <summary>
    /// 任务奖励的钱钱数量
    /// </summary>
    public decimal RewardsMoneyNumber { get; set; }

    public DateTime CreationTime{ get; set; }
    public int OrderNum { get; set; }
}