using Hangfire.Server;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

public class UnitOfWorkHangfireFilter : IServerFilter, ISingletonDependency
{
    private const string CurrentJobUow = "HangfireUnitOfWork";
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UnitOfWorkHangfireFilter(IUnitOfWorkManager unitOfWorkManager)
    {
        _unitOfWorkManager = unitOfWorkManager;
    }

    public void OnPerforming(PerformingContext context)
    {
        var uow = _unitOfWorkManager.Begin();
        context.Items.Add(CurrentJobUow, uow);
    }

    public void OnPerformed(PerformedContext context)
    {
        AsyncHelper.RunSync(()=>OnPerformedAsync(context));
    }

    private async Task OnPerformedAsync(PerformedContext context)
    {
        if (context.Items.TryGetValue(CurrentJobUow, out var obj)
            && obj is IUnitOfWork uow)
        {
            if (context.Exception == null && !uow.IsCompleted)
            {
                await uow.CompleteAsync();
            }
            else
            {
                await uow.RollbackAsync();
            }
            uow.Dispose();
        }
    }
}