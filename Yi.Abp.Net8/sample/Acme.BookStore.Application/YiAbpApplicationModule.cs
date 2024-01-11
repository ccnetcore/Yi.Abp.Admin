using Volo.Abp.Modularity;
using Acme.BookStore.Application.Contracts;
using Acme.BookStore.Domain;
using Yi.Framework.Bbs.Application;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application;

namespace Acme.BookStore.Application
{
    [DependsOn(
        typeof(YiAbpApplicationContractsModule),
        typeof(YiAbpDomainModule),
       

        typeof(YiFrameworkRbacApplicationModule),
         typeof(YiFrameworkBbsApplicationModule),

        typeof(YiFrameworkDddApplicationModule)
        )]
    public class YiAbpApplicationModule : AbpModule
    {
    }
}
