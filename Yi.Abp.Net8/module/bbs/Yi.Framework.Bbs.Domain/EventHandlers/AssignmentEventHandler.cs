using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities.Assignment;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers;

/// <summary>
/// 任务系统的领域事件，处理不同任务触发变化
/// </summary>
public class AssignmentEventHandler : ILocalEventHandler<AssignmentEventArgs>, ITransientDependency
{
    private readonly ISqlSugarRepository<AssignmentAggregateRoot> _repository;

    public AssignmentEventHandler(ISqlSugarRepository<AssignmentAggregateRoot> repository)
    {
        _repository = repository;
    }

    public async Task HandleEventAsync(AssignmentEventArgs eventData)
    {
        var currentAssignmentList = await _repository.GetListAsync(x =>
            x.AssignmentState == AssignmentStateEnum.Progress && x.UserId == eventData.CurrentUserId);

        //如果有接收的任务
        if (currentAssignmentList.Count > 0)
        {
            switch (eventData.RequirementType)
            {
                //发表主题
                case AssignmentRequirementTypeEnum.Discuss:
                    SetCurrentStepNumber(AssignmentRequirementTypeEnum.Discuss, currentAssignmentList);
                    break;

                //发表评论
                case AssignmentRequirementTypeEnum.Comment:
                    SetCurrentStepNumber(AssignmentRequirementTypeEnum.Comment, currentAssignmentList);
                    break;

                //点赞
                case AssignmentRequirementTypeEnum.Agree:
                    SetCurrentStepNumber(AssignmentRequirementTypeEnum.Agree, currentAssignmentList);
                    break;

                //更新昵称
                case AssignmentRequirementTypeEnum.UpdateNick:
                    SetCurrentStepNumber(AssignmentRequirementTypeEnum.UpdateNick, currentAssignmentList);
                    break;

                //更新头像
                case AssignmentRequirementTypeEnum.UpdateIcon:
                    SetCurrentStepNumber(AssignmentRequirementTypeEnum.UpdateIcon, currentAssignmentList);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


            //更新
            await _repository.UpdateRangeAsync(currentAssignmentList);
        }
    }

    /// <summary>
    /// 设置当前进度
    /// </summary>
    /// <param name="requirementType"></param>
    /// <param name="currentAssignmentList"></param>
    private void SetCurrentStepNumber(AssignmentRequirementTypeEnum requirementType,
        List<AssignmentAggregateRoot> currentAssignmentList)
    {
        currentAssignmentList.ForEach(x =>
        {
            if (x.AssignmentRequirementType == requirementType &&
                x.CurrentStepNumber < x.TotalStepNumber)
            {
                x.CurrentStepNumber += 1;
                if (x.CurrentStepNumber == x.TotalStepNumber)
                {
                    x.AssignmentState = AssignmentStateEnum.Completed;
                }
            }
        });
    }
}