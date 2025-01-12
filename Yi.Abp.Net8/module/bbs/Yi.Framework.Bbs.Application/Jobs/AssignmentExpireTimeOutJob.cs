using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.Bbs.Domain.Managers;

namespace Yi.Framework.Bbs.Application.Jobs;

/// <summary>
/// 每日任务job
/// </summary>
public class AssignmentExpireTimeOutJob : HangfireBackgroundWorkerBase
{
    private readonly AssignmentManager _assignmentManager;

    public AssignmentExpireTimeOutJob(AssignmentManager assignmentManager)
    {
        _assignmentManager = assignmentManager;
        
        RecurringJobId = "每日任务系统超时检测";
        //每分钟执行一次
        CronExpression = "0 * * * * ?";
        //
        // JobDetail = JobBuilder.Create<AssignmentExpireTimeOutJob>().WithIdentity(nameof(AssignmentExpireTimeOutJob)).Build();
        // //每个小时整点执行一次
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AssignmentExpireTimeOutJob)).WithCronSchedule("0 0 * * * ?")
        //     .Build();
    }
    
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await _assignmentManager.ExpireTimeoutAsync();
    }
}