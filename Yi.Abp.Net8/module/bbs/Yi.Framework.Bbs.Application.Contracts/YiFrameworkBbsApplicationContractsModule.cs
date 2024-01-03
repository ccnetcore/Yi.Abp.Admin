using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.Rbac.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts
{
    [DependsOn(typeof(YiFrameworkBbsDomainSharedModule),

        typeof(YiFrameworkRbacApplicationContractsModule)
        )]
    public class YiFrameworkBbsApplicationContractsModule : AbpModule
    {

    }
}