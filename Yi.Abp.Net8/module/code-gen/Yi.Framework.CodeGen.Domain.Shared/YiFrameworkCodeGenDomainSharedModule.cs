using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace Yi.Framework.CodeGen.Domain.Shared
{
    [DependsOn(typeof(AbpDddDomainSharedModule))]
    public class YiFrameworkCodeGenDomainSharedModule : AbpModule
    {

    }
}
