using Volo.Abp.Modularity;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Domain.Shared;

namespace Yi.Framework.Rbac.Application.Contracts
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainSharedModule),


        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiFrameworkRbacApplicationContractsModule : AbpModule
    {

    }
}