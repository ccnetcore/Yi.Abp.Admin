using Yi.Framework.DigitalCollectibles.Application.Contracts;
using Yi.Framework.DigitalCollectibles.Domain;
using Yi.Framework.Ddd.Application;


namespace Yi.Framework.DigitalCollectibles.Application
{
    [DependsOn(
        typeof(YiFrameworkDigitalCollectiblesApplicationContractsModule),
        typeof(YiFrameworkDigitalCollectiblesDomainModule),
        
        typeof(YiFrameworkDddApplicationModule)
        )]
    public class YiFrameworkDigitalCollectiblesApplicationModule : AbpModule
    {
    }
}
