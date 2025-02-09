using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.DigitalCollectibles.Domain.Managers;

namespace Yi.Framework.DigitalCollectibles.Application.Jobs;

/// <summary>
/// 处理挂机挖矿定时任务
/// </summary>
public class OnHookAutoMiningJob : HangfireBackgroundWorkerBase
{
    private readonly MiningPoolManager _miningPoolManager;
    private readonly ILogger<OnHookAutoMiningJob> _logger;

    public OnHookAutoMiningJob(MiningPoolManager miningPoolManager, ILogger<OnHookAutoMiningJob> logger)
    {
        _miningPoolManager = miningPoolManager;
        _logger = logger;
        
        RecurringJobId = "自动挂机挖矿";
        //每小时执行一次
        CronExpression = "0 0 * * * ?";
        //
        // JobDetail = JobBuilder.Create<OnHookAutoMiningJob>().WithIdentity(nameof(OnHookAutoMiningJob))
        //     .Build();
        //
        // //每小时执行一次
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(OnHookAutoMiningJob))
        //     // .WithCronSchedule("10 * * * * ?")
        //     .WithCronSchedule("0 0 * * * ?")
        //     .Build();
    }
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await _miningPoolManager.OnHookMiningAsync();
    }
}