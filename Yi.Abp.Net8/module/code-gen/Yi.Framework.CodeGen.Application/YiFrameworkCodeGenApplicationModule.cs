using Yi.Framework.CodeGen.Application.Contracts;
using Yi.Framework.CodeGen.Domain;
using Yi.Framework.Ddd.Application;

namespace Yi.Framework.CodeGen.Application
{
    [DependsOn(typeof(YiFrameworkCodeGenApplicationContractsModule),
        typeof(YiFrameworkCodeGenDomainModule),
        typeof(YiFrameworkDddApplicationModule))]
    public class YiFrameworkCodeGenApplicationModule : AbpModule
    {

    }
}
