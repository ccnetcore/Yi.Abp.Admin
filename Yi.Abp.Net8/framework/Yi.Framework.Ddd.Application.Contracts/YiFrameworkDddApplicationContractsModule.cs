using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Yi.Framework.Ddd.Application.Contracts
{
    [DependsOn(typeof(AbpDddApplicationContractsModule))]
    public class YiFrameworkDddApplicationContractsModule : AbpModule
    {
    }
}
