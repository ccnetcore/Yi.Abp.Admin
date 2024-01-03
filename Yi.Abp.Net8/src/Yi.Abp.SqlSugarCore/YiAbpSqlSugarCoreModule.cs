using Volo.Abp.Modularity;
using Yi.Abp.Domain;
using Yi.Abp.SqlSugarCore;
using Yi.Framework.Bbs.SqlSugarCore;
using Yi.Framework.Mapster;
using Yi.Framework.Rbac.SqlSugarCore;
using Yi.Framework.SqlSugarCore;

namespace Yi.Abp.SqlsugarCore
{
    [DependsOn(
        typeof(YiAbpDomainModule),

        typeof(YiFrameworkRbacSqlSugarCoreModule),
        typeof(YiFrameworkBbsSqlSugarCoreModule),

        typeof(YiFrameworkMapsterModule),
        typeof(YiFrameworkSqlSugarCoreModule)
        )]
    public class YiAbpSqlSugarCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddYiDbContext<YiDbContext>();
        }
    }
}