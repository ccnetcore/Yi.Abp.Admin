using Volo.Abp.Caching;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Acme.BookStore.Domain.Shared;
using Yi.Framework.Bbs.Domain;
using Yi.Framework.Mapster;
using Yi.Framework.Rbac.Domain;

namespace Acme.BookStore.Domain
{
    [DependsOn(
        typeof(YiAbpDomainSharedModule),
       

        typeof(YiFrameworkRbacDomainModule),
        typeof(YiFrameworkBbsDomainModule),

        typeof(YiFrameworkMapsterModule),
        typeof(AbpDddDomainModule),
        typeof(AbpCachingModule)
        )]
    public class YiAbpDomainModule : AbpModule
    {

    }
}