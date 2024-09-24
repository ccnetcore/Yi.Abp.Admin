using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Assignment;

public class AssignmentGetListInput
{
    /// <summary>
    /// 任务查询条件
    /// </summary>
    public AssignmentQueryStateEnum AssignmentQueryState { get; set; } = AssignmentQueryStateEnum.Progress;
}

public enum AssignmentQueryStateEnum
{
    /// <summary>
    /// 正在进行
    /// </summary>
    Progress,
    
    /// <summary>
    /// 已结束
    /// </summary>
    End
}