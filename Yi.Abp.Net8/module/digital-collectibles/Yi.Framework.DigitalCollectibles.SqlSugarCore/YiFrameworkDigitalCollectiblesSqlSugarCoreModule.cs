using Yi.Framework.DigitalCollectibles.Domain;
using Yi.Framework.Mapster;
using Yi.Framework.SettingManagement.SqlSugarCore;
using Yi.Framework.SqlSugarCore;

namespace Yi.Framework.DigitalCollectibles.SqlsugarCore
{
    [DependsOn(
        typeof(YiFrameworkDigitalCollectiblesDomainModule),
        typeof(YiFrameworkSettingManagementSqlSugarCoreModule),
        typeof(YiFrameworkMapsterModule),
        typeof(YiFrameworkSqlSugarCoreModule)
        )]
    public class YiFrameworkDigitalCollectiblesSqlSugarCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}