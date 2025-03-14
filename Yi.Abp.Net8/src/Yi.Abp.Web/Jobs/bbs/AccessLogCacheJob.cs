using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Shared.Etos;

namespace Yi.Abp.Web.Jobs.bbs;

public class AccessLogCacheJob : HangfireBackgroundWorkerBase
{
    private readonly ILocalEventBus _localEventBus;

    public AccessLogCacheJob(ILocalEventBus localEventBus)
    {
        _localEventBus = localEventBus;
        RecurringJobId = "访问日志写入缓存";
        //每分钟执行一次，将本地缓存转入redis，防止丢数据
        CronExpression = "0 * * * * ?";
        //
        // JobDetail = JobBuilder.Create<AccessLogCacheJob>().WithIdentity(nameof(AccessLogCacheJob))
        //     .Build();

        //每10秒执行一次，将本地缓存转入redis，防止丢数据
        // Trigger = TriggerBuilder.Create().WithIdentity(nameof(AccessLogCacheJob))
        //     .WithSimpleSchedule((schedule) => { schedule.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever();; })
        //     .Build();
    }
    
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await _localEventBus.PublishAsync(new AccessLogResetArgs());
    }
}