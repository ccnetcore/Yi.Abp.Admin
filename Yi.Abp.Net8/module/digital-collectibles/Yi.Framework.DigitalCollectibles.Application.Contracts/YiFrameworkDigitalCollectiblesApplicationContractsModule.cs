using Yi.Framework.DigitalCollectibles.Domain.Shared;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts
{
    [DependsOn(
        typeof(YiFrameworkDigitalCollectiblesDomainSharedModule),
        
        typeof(YiFrameworkRbacApplicationContractsModule),
        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiFrameworkDigitalCollectiblesApplicationContractsModule:AbpModule
    {

    }
}