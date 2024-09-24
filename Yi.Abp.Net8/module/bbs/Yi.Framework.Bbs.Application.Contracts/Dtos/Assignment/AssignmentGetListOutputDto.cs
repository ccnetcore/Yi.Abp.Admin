using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Assignment;

public class AssignmentGetListOutputDto:EntityDto<Guid>
{
    /// <summary>
    /// 任务名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remarks { get; set; }
    /// <summary>
    /// 当前步骤数
    /// </summary>
    public int CurrentStepNumber { get; set; }

    /// <summary>
    /// 总共步骤数
    /// </summary>
    public int TotalStepNumber { get; set; }
    /// <summary>
    /// 任务类型
    /// </summary>
    public AssignmentTypeEnum AssignmentType { get; set; }
    
    /// <summary>
    /// 任务需求类型
    /// </summary>
    public AssignmentRequirementTypeEnum AssignmentRequirementType{ get; set; }
    /// <summary>
    /// 任务状态
    /// </summary>
    public AssignmentStateEnum AssignmentState { get; set; }

    /// <summary>
    /// 任务奖励的钱钱数量
    /// </summary>
    public decimal RewardsMoneyNumber { get; set; }
    /// <summary>
    /// 任务过期时间
    /// </summary>
    public DateTime? ExpireTime { get; set; }

    public DateTime? CompleteTime { get; set; }
    
    
    public DateTime CreationTime { get; set; }
    public int OrderNum { get; set; }
}