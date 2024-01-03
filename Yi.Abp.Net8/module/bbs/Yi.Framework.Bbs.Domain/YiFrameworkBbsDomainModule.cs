using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.Rbac.Domain;

namespace Yi.Framework.Bbs.Domain
{
    [DependsOn(
        typeof(YiFrameworkBbsDomainSharedModule),

        typeof(YiFrameworkRbacDomainModule)
        )]
    public class YiFrameworkBbsDomainModule : AbpModule
    {

    }
}