using Yi.Framework.CodeGen.Domain.Shared;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.CodeGen.Application.Contracts
{
    [DependsOn(typeof(YiFrameworkCodeGenDomainSharedModule),
        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiFrameworkCodeGenApplicationContractsModule : AbpModule
    {

    }
}
