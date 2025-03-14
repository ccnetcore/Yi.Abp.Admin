using Hangfire.Server;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

/// <summary>
/// Hangfire 工作单元过滤器
/// 用于管理后台任务的事务处理
/// </summary>
public sealed class UnitOfWorkHangfireFilter : IServerFilter, ISingletonDependency
{
    private const string UnitOfWorkItemKey = "HangfireUnitOfWork";
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    /// <summary>
    /// 初始化工作单元过滤器
    /// </summary>
    /// <param name="unitOfWorkManager">工作单元管理器</param>
    public UnitOfWorkHangfireFilter(IUnitOfWorkManager unitOfWorkManager)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    /// <summary>
    /// 任务执行前的处理
    /// </summary>
    /// <param name="context">执行上下文</param>
    public void OnPerforming(PerformingContext context)
    {
        // 开启一个工作单元并存储到上下文中
        var uow = _unitOfWorkManager.Begin();
        context.Items.Add(UnitOfWorkItemKey, uow);
    }

    /// <summary>
    /// 任务执行后的处理
    /// </summary>
    /// <param name="context">执行上下文</param>
    public void OnPerformed(PerformedContext context)
    {
        AsyncHelper.RunSync(() => OnPerformedAsync(context));
    }

    /// <summary>
    /// 任务执行后的异步处理
    /// </summary>
    /// <param name="context">执行上下文</param>
    private async Task OnPerformedAsync(PerformedContext context)
    {
        if (!context.Items.TryGetValue(UnitOfWorkItemKey, out var obj) || 
            obj is not IUnitOfWork uow)
        {
            return;
        }

        try
        {
            // 如果没有异常且工作单元未完成，则提交事务
            if (context.Exception == null && !uow.IsCompleted)
            {
                await uow.CompleteAsync();
            }
            else
            {
                // 否则回滚事务
                await uow.RollbackAsync();
            }
        }
        finally
        {
            // 确保工作单元被释放
            uow.Dispose();
        }
    }
}