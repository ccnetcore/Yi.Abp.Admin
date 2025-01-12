using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.DependencyInjection;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

public class YiHangfireConventionalRegistrar : DefaultConventionalRegistrar
{
    protected override bool IsConventionalRegistrationDisabled(Type type)
    {
        return !typeof(IHangfireBackgroundWorker).IsAssignableFrom(type) || base.IsConventionalRegistrationDisabled(type);
    }

    protected override List<Type> GetExposedServiceTypes(Type type)
    {
        return new List<Type>()
            {
                typeof(IHangfireBackgroundWorker)
            };
    }
}
