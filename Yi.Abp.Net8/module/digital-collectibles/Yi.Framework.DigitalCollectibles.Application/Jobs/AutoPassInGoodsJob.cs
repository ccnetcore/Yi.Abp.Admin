using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.DigitalCollectibles.Domain.Managers;

namespace Yi.Framework.DigitalCollectibles.Application.Jobs;
/// <summary>
/// 自动下架商品
/// </summary>
public class AutoPassInGoodsJob: HangfireBackgroundWorkerBase
{
    private readonly MarketManager _marketManager;
    private readonly ILogger<AutoPassInGoodsJob> _logger;
    public AutoPassInGoodsJob(MarketManager marketManager, ILogger<AutoPassInGoodsJob> logger)
    {
        _marketManager = marketManager;
        _logger = logger;
        RecurringJobId = "交易市场自动流拍";
        //每小时,第10分钟执行一次
        CronExpression = "0 10 * * * ?";
        //
        // JobDetail = JobBuilder.Create<AutoPassInGoodsJob>().WithIdentity(nameof(AutoPassInGoodsJob))
        //     .Build();
        //
        // //每小时,第10分钟执行一次
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AutoPassInGoodsJob))
        //     // .WithSimpleSchedule((builer) =>
        //     // {
        //     //     builer.WithIntervalInHours(10);
        //     // })
        //     // .StartNow()
        //     .WithCronSchedule("0 10 * * * ?")
        //     .Build();
    }
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await  _marketManager.AutoPassInGoodsAsync();
    }
}