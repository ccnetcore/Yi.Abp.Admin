using System.Linq.Expressions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.DynamicProxy;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

[DependsOn(typeof(AbpBackgroundWorkersHangfireModule))]
public class YiFrameworkBackgroundWorkersHangfireModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddConventionalRegistrar(new YiHangfireConventionalRegistrar());
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        //定时任务自动注入，Abp默认只有在Quartz才实现
        var backgroundWorkerManager = context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>();
        var works = context.ServiceProvider.GetServices<IHangfireBackgroundWorker>();

        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        //【特殊，为了兼容内存模式，由于内存模式任务，不能使用队列】
        bool.TryParse(configuration["Redis:IsEnabled"], out var redisEnabled);
        foreach (var work in works)
        {
            //如果为空，默认使用服务器本地上海时间
            work.TimeZone = TimeZoneInfo.Local;
            if (redisEnabled)
            {
                await backgroundWorkerManager.AddAsync(work);
            }
            else
            {
                object unProxyWorker = ProxyHelper.UnProxy((object)work);
                RecurringJob.AddOrUpdate(work.RecurringJobId,
                    (Expression<Func<Task>>)(() =>
                        ((IHangfireBackgroundWorker)unProxyWorker).DoWorkAsync(default(CancellationToken))),
                    work.CronExpression, new RecurringJobOptions()
                    {
                        TimeZone = work.TimeZone
                    });
            }
        }
    }

    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var services = context.ServiceProvider;
        GlobalJobFilters.Filters.Add(services.GetRequiredService<UnitOfWorkHangfireFilter>());
    }
}