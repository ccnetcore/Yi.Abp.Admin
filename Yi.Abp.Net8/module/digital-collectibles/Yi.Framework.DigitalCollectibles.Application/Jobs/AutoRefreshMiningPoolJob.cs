using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.DigitalCollectibles.Domain.Managers;

namespace Yi.Framework.DigitalCollectibles.Application.Jobs;

/// <summary>
/// 自动刷新填满矿池
/// </summary>
public class AutoRefreshMiningPoolJob : HangfireBackgroundWorkerBase
{
    private readonly MiningPoolManager _miningPoolManager;

    public AutoRefreshMiningPoolJob(MiningPoolManager miningPoolManager)
    {
        _miningPoolManager = miningPoolManager;
        
        RecurringJobId = "刷新矿池和用户限制";
        //每天早上10点执行一次
        CronExpression = "0 0 10 * * ?";
        //
        // JobDetail = JobBuilder.Create<AutoRefreshMiningPoolJob>().WithIdentity(nameof(AutoRefreshMiningPoolJob))
        //     .Build();
        //
        // //每天早上10点执行一次
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AutoRefreshMiningPoolJob))
        //     .WithCronSchedule("0 0 10 * * ?")
        //     .Build();
        //
        
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AutoRefreshMiningPoolJob))
        //     .WithSimpleSchedule((schedule) =>
        //     {
        //         schedule.WithInterval(TimeSpan.FromHours(1));
        //     })
        //     .StartNow()
        //     .Build();
    }
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
   
        //刷新矿池
        await _miningPoolManager.RefreshMiningPoolAsync();
        //刷新用户限制
        await _miningPoolManager.RefreshMiningUserLimitAsync();
    }
}