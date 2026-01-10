using Yi.Abp.Application.Contracts;
using Yi.Abp.Domain;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application;
using Yi.Framework.SettingManagement.Application;
using Yi.Framework.TenantManagement.Application;

namespace Yi.Abp.Application
{
    [DependsOn(
        typeof(YiAbpApplicationContractsModule),
        typeof(YiAbpDomainModule),


        typeof(YiFrameworkRbacApplicationModule),
        
        typeof(YiFrameworkTenantManagementApplicationModule),
        typeof (YiFrameworkSettingManagementApplicationModule),

        typeof(YiFrameworkDddApplicationModule)
        )]
    public class YiAbpApplicationModule : AbpModule
    {
    }
}
