using Volo.Abp.Modularity;
using Yi.Framework.Rbac.Domain.Shared;

namespace Yi.Framework.Bbs.Domain.Shared
{
    [DependsOn(        
        typeof(YiFrameworkRbacDomainSharedModule))]
    public class YiFrameworkBbsDomainSharedModule : AbpModule
    {

    }
}