using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Yi.Framework.SettingManagement.Domain;
using Yi.Framework.SqlSugarCore;

namespace Yi.Framework.SettingManagement.SqlSugarCore
{
    [DependsOn(
        typeof(YiFrameworkSettingManagementDomainModule),
        typeof(YiFrameworkSqlSugarCoreModule)
        )]
    public class YiFrameworkSettingManagementSqlSugarCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            services.AddTransient<ISettingRepository, SqlSugarCoreSettingRepository>();
        }
    }
}
