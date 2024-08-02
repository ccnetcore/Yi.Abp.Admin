using Volo.Abp.SettingManagement;
using Yi.Abp.Application.Contracts;
using Yi.Abp.Domain;
using Yi.Framework.Bbs.Application;
using Yi.Framework.ChatHub.Application;
using Yi.Framework.CodeGen.Application;
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
         typeof(YiFrameworkBbsApplicationModule),
         typeof(YiFrameworkChatHubApplicationModule),
        typeof(YiFrameworkTenantManagementApplicationModule),
        typeof(YiFrameworkCodeGenApplicationModule),
        typeof (YiFrameworkSettingManagementApplicationModule),

        typeof(YiFrameworkDddApplicationModule)
        )]
    public class YiAbpApplicationModule : AbpModule
    {
    }
}
