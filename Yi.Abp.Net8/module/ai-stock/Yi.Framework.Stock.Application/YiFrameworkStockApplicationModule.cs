using Yi.Framework.Stock.Application.Contracts;
using Yi.Framework.Stock.Domain;
using Yi.Framework.Ddd.Application;

namespace Yi.Framework.Stock.Application
{
    [DependsOn(
        typeof(YiFrameworkStockApplicationContractsModule),
        typeof(YiFrameworkStockDomainModule),
        
        typeof(YiFrameworkDddApplicationModule)

    )]
    public class YiFrameworkStockApplicationModule : AbpModule
    {
    }
}