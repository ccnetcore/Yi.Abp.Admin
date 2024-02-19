using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Framework.CodeGen.Domain.Shared;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.Domain
{
    [DependsOn(typeof(YiFrameworkCodeGenDomainSharedModule),
        typeof(AbpDddDomainModule),
        typeof(YiFrameworkSqlSugarCoreAbstractionsModule))]
    public class YiFrameworkCodeGenDomainModule : AbpModule
    {

    }
}
