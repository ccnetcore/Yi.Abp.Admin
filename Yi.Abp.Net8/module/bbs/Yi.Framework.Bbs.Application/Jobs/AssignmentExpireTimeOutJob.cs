using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Yi.Framework.Bbs.Domain.Managers;

namespace Yi.Framework.Bbs.Application.Jobs;

/// <summary>
/// 每日任务job
/// </summary>
public class AssignmentExpireTimeOutJob : QuartzBackgroundWorkerBase
{
    private readonly AssignmentManager _assignmentManager;

    public AssignmentExpireTimeOutJob(AssignmentManager assignmentManager)
    {
        _assignmentManager = assignmentManager;
        JobDetail = JobBuilder.Create<AssignmentExpireTimeOutJob>().WithIdentity(nameof(AssignmentExpireTimeOutJob)).Build();
        //每个小时整点执行一次
        Trigger = TriggerBuilder.Create().WithIdentity(nameof(AssignmentExpireTimeOutJob)).WithCronSchedule("0 0 * * * ?")
            .Build();
    }

    public override async Task Execute(IJobExecutionContext context)
    {
        await _assignmentManager.ExpireTimeoutAsync();
    }
}