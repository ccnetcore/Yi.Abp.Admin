using Volo.Abp.Application;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;
using Volo.Abp.Timing;
using Yi.Framework.SettingManagement.Domain;

namespace Yi.Framework.SettingManagement.Application;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(AbpSettingManagementApplicationContractsModule),
    typeof(YiFrameworkSettingManagementDomainModule),
    typeof(AbpTimingModule)

)]
public class YiFrameworkSettingManagementApplicationModule : AbpModule
{
}
