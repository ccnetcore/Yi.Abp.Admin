using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Domain;
using Yi.Framework.Rbac.SqlSugarCore;

namespace Yi.Framework.Bbs.SqlSugarCore
{
    [DependsOn(
        typeof(YiFrameworkBbsDomainModule),

        typeof(YiFrameworkRbacSqlSugarCoreModule))]
    public class YiFrameworkBbsSqlSugarCoreModule : AbpModule
    {

    }
}