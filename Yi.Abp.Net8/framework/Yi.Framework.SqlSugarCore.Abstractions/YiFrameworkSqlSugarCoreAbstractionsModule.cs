using Volo.Abp.Modularity;
using Yi.Framework.Core;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    [DependsOn(typeof(YiFrameworkCoreModule))]
    public class YiFrameworkSqlSugarCoreAbstractionsModule : AbpModule
    {

    }
}