using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;

namespace YiFrameworkTenantManagementDomain.Shared
{
    [DependsOn(typeof(AbpTenantManagementDomainSharedModule))]
    public class YiFrameworkTenantManagementDomainSharedModule : AbpModule
    {

    }
}
