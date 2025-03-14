using System.Linq.Expressions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.DynamicProxy;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

/// <summary>
/// Hangfire 后台任务模块
/// </summary>
[DependsOn(typeof(AbpBackgroundWorkersHangfireModule),
    typeof(AbpBackgroundJobsHangfireModule))]
public sealed class YiFrameworkBackgroundWorkersHangfireModule : AbpModule
{
    /// <summary>
    /// 配置服务前的预处理
    /// </summary>
    /// <param name="context">服务配置上下文</param>
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        // 添加 Hangfire 后台任务约定注册器
        context.Services.AddConventionalRegistrar(new YiHangfireConventionalRegistrar());
    }

    /// <summary>
    /// 应用程序初始化
    /// </summary>
    /// <param name="context">应用程序初始化上下文</param>
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // 获取后台任务管理器和所有 Hangfire 后台任务
        var backgroundWorkerManager = context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>();
        var workers = context.ServiceProvider.GetServices<IHangfireBackgroundWorker>();

        // 获取配置
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
        
        // 检查是否启用 Redis
        var isRedisEnabled = configuration.GetValue<bool>("Redis:IsEnabled");

        foreach (var worker in workers)
        {
            // 设置时区为本地时区(上海)
            worker.TimeZone = TimeZoneInfo.Local;

            if (isRedisEnabled)
            {
                // Redis 模式：使用 ABP 后台任务管理器
                await backgroundWorkerManager.AddAsync(worker);
            }
            else
            {
                // 内存模式：直接使用 Hangfire
                var unProxyWorker = ProxyHelper.UnProxy(worker);
                
                // 添加或更新循环任务
                RecurringJob.AddOrUpdate(
                    worker.RecurringJobId,
                    (Expression<Func<Task>>)(() => 
                        ((IHangfireBackgroundWorker)unProxyWorker).DoWorkAsync(default)),
                    worker.CronExpression,
                    new RecurringJobOptions
                    {
                        TimeZone = worker.TimeZone
                    });
            }
        }
    }

    /// <summary>
    /// 应用程序初始化前的预处理
    /// </summary>
    /// <param name="context">应用程序初始化上下文</param>
    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        // 添加工作单元过滤器
        var services = context.ServiceProvider;
        GlobalJobFilters.Filters.Add(services.GetRequiredService<UnitOfWorkHangfireFilter>());
    }
}