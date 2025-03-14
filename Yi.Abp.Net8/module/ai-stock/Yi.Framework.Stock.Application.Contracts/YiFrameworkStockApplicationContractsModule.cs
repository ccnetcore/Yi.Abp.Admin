using Volo.Abp.SettingManagement;
using Yi.Framework.Stock.Domain.Shared;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Stock.Application.Contracts
{
    [DependsOn(
        typeof(YiFrameworkStockDomainSharedModule),

        typeof(AbpSettingManagementApplicationContractsModule),

        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiFrameworkStockApplicationContractsModule:AbpModule
    {

    }
}