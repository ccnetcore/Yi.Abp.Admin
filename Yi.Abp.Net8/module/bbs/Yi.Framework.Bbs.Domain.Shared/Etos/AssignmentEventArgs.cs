using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Shared.Etos;

public class AssignmentEventArgs
{
    public AssignmentEventArgs(AssignmentRequirementTypeEnum requirementType, Guid currentUserId,object? args=null)
    {
        RequirementType = requirementType;
        Args = args;
        CurrentUserId = currentUserId;
    }

    /// <summary>
    /// 任务需求类型
    /// </summary>
    public AssignmentRequirementTypeEnum RequirementType { get; set; }


    /// <summary>
    /// 任务参数，可空，只需要一个触发点即可
    /// </summary>
    public object? Args { get; set; }


    /// <summary>
    /// 当前用户id
    /// </summary>
    public Guid CurrentUserId { get; set; }
}