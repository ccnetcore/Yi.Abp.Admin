using SqlSugar;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Domain.Entities.Assignment;
using Yi.Framework.Bbs.Domain.Managers.AssignmentProviders;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers;

/// <summary>
/// 任务领域，任务相关核心逻辑
/// </summary>
public class AssignmentManager : DomainService
{
    private readonly IEnumerable<IAssignmentProvider> _assignmentProviders;
    public readonly ISqlSugarRepository<AssignmentAggregateRoot> _assignmentRepository;
    public readonly ISqlSugarRepository<AssignmentDefineAggregateRoot> _assignmentDefineRepository;
    private readonly ILocalEventBus _localEventBus;

    public AssignmentManager(IEnumerable<IAssignmentProvider> assignmentProviders,
        ISqlSugarRepository<AssignmentAggregateRoot> assignmentRepository,
        ISqlSugarRepository<AssignmentDefineAggregateRoot> assignmentDefineRepository, ILocalEventBus localEventBus)
    {
        this._assignmentProviders = assignmentProviders;
        _assignmentRepository = assignmentRepository;
        _assignmentDefineRepository = assignmentDefineRepository;
        _localEventBus = localEventBus;
    }

    /// <summary>
    /// 接受任务
    /// </summary>
    /// <param name="userId">领取用户</param>
    /// <param name="asignmentDefineId">任务定义id</param>
    /// <returns></returns>
    public async Task AcceptAsync(Guid userId, Guid asignmentDefineId)
    {
        var canReceiveList = await GetCanReceiveListAsync(userId);

        //如果要接收的任务id在可领取的任务列表中，就可以接收任务
        if (canReceiveList.Select(x => x.Id).Contains(asignmentDefineId))
        {
            var assignmentDefine = await _assignmentDefineRepository.GetByIdAsync(asignmentDefineId);

            var entity = new AssignmentAggregateRoot();
            entity.AssignmentDefineId = asignmentDefineId;
            entity.UserId = userId;
            entity.AssignmentState = AssignmentStateEnum.Progress;
            entity.CurrentStepNumber = 0;
            entity.TotalStepNumber = assignmentDefine.TotalStepNumber;
            entity.RewardsMoneyNumber = assignmentDefine.RewardsMoneyNumber;
            entity.AssignmentRequirementType = assignmentDefine.AssignmentRequirementType;
            entity.ExpireTime = assignmentDefine.AssignmentType.GetExpireTime();
            await _assignmentRepository.InsertAsync(entity);
        }
    }

    /// <summary>
    /// 领取任务奖励
    /// </summary>
    /// <param name="asignmentId">任务id</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ReceiveRewardsAsync(Guid asignmentId)
    {
        var assignment = await _assignmentRepository.GetByIdAsync(asignmentId);
        if (assignment.IsAllowCompleted())
        {
            //加钱加钱
            await _localEventBus.PublishAsync(
                new MoneyChangeEventArgs { UserId = assignment.UserId, Number = assignment.RewardsMoneyNumber }, false);

            //设置已完成，并领取奖励，钱钱
            assignment.SetEnd();
            await _assignmentRepository.UpdateAsync(assignment);
        }
        else
        {
            //不能领取
            throw new UserFriendlyException("该任务没有满足领取条件，请检查任务详情");
        }
    }


    /// <summary>
    /// 根据用户id获取能够领取的任务列表
    /// </summary>
    /// <param name="userId">用户id</param>
    /// <returns></returns>
    public async Task<List<AssignmentDefineAggregateRoot>> GetCanReceiveListAsync(Guid userId)
    {
        var context = await GetAssignmentContext(userId);
        var output = new List<AssignmentDefineAggregateRoot>();
        foreach (var assignmentProvider in _assignmentProviders)
        {
            output.AddRange(await assignmentProvider.GetCanReceiveListAsync(context));
        }

        output = output.DistinctBy(x => x.Id).OrderBy(x => x.OrderNum).ToList();
        return output;
    }


    /// <summary>
    /// 获取任务的上下文
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<AssignmentContext> GetAssignmentContext(Guid userId)
    {
        var allAssignmentDefine = await _assignmentDefineRepository.GetListAsync();

        var currentUserAssignment = await _assignmentRepository.GetListAsync(x => x.UserId == userId);

        var context = new AssignmentContext(userId, allAssignmentDefine, currentUserAssignment);
        return context;
    }


    /// <summary>
    /// 过期超时的任务,定时任务去处理即可
    /// </summary>
    public async Task ExpireTimeoutAsync()
    {
        var progressEntities = await _assignmentRepository._DbQueryable
            .Where(x => x.AssignmentState == AssignmentStateEnum.Progress)
            .ToListAsync();

        var needUpdateEntities = new List<AssignmentAggregateRoot>();
        foreach (var progressEntity in progressEntities)
        {
            if (progressEntity.TrySetExpire())
            {
                needUpdateEntities.Add(progressEntity);
            }
        }

        if (needUpdateEntities.Any())
        {
            await _assignmentRepository._Db.Updateable(needUpdateEntities).ExecuteCommandAsync();
        }
    }
}