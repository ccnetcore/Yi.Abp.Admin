using Volo.Abp.Modularity;
using Yi.Framework.Mapster;
using Yi.Framework.Rbac.Domain;
using Yi.Framework.SqlSugarCore;

namespace Yi.Framework.Rbac.SqlSugarCore
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainModule),

        typeof(YiFrameworkMapsterModule),
        typeof(YiFrameworkSqlSugarCoreModule)
        )]
    public class YiFrameworkRbacSqlSugarCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.TryAddYiDbContext<YiRbacDbContext>();
        }
    }
}