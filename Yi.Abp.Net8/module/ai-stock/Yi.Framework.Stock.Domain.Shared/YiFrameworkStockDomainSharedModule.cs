using Volo.Abp.Domain;
using Volo.Abp.SettingManagement;

namespace Yi.Framework.Stock.Domain.Shared
{
    [DependsOn(
        
        typeof(AbpSettingManagementDomainSharedModule),
        typeof(AbpDddDomainSharedModule))]
    public class YiFrameworkStockDomainSharedModule : AbpModule
    {

    }
}