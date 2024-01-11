using Volo.Abp.Modularity;
using Acme.BookStore.Domain.Shared;
using Yi.Framework.Bbs.Application.Contracts;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts;

namespace Acme.BookStore.Application.Contracts
{
    [DependsOn(
        typeof(YiAbpDomainSharedModule),

        typeof(YiFrameworkRbacApplicationContractsModule),
        typeof(YiFrameworkBbsApplicationContractsModule),

        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiAbpApplicationContractsModule:AbpModule
    {

    }
}