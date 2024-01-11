using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.Rbac.Domain.Shared;

namespace Acme.BookStore.Domain.Shared
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainSharedModule),
        typeof(YiFrameworkBbsDomainSharedModule),
        
        typeof(AbpDddDomainSharedModule))]
    public class YiAbpDomainSharedModule : AbpModule
    {

    }
}