using Volo.Abp.Modularity;
using Yi.Framework.TenantManagement.Domain;

namespace Yi.Framework.TenantManagement.SqlSugarCore
{
    [DependsOn(typeof(YiFrameworkTenantManagementDomainModule))]
    public class YiFrameworkTenantManagementSqlSugarCoreModule : AbpModule
    {
    }
}
