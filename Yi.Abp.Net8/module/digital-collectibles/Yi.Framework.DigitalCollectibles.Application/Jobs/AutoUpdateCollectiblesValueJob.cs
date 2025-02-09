using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.DigitalCollectibles.Domain.Managers;

namespace Yi.Framework.DigitalCollectibles.Application.Jobs;

/// <summary>
/// 自动更新藏品价值
/// </summary>
public class AutoUpdateCollectiblesValueJob : HangfireBackgroundWorkerBase
{
    private readonly CollectiblesManager _collectiblesManager;

    public AutoUpdateCollectiblesValueJob(CollectiblesManager collectiblesManager)
    {
        _collectiblesManager = collectiblesManager;
        RecurringJobId = "更新藏品价值";
        //每天早上9点执行一次
        CronExpression = "0 0 9 * * ?";
    }

    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await _collectiblesManager.UpdateAllValueAsync();
    }
}