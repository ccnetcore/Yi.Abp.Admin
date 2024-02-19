using Volo.Abp.Modularity;
using Yi.Framework.Ddd.Application;
using Yi.Framework.TenantManagement.Domain;

namespace Yi.Framework.TenantManagement.Application
{
    [DependsOn(typeof(YiFrameworkTenantManagementDomainModule))]
    public class YiFrameworkTenantManagementApplicationModule: AbpModule
    {

    }
}
