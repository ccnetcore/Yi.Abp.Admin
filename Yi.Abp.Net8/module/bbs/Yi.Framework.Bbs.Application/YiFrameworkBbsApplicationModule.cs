using Volo.Abp.Modularity;
using Yi.Framework.Bbs.Application.Contracts;
using Yi.Framework.Bbs.Domain;
using Yi.Framework.Rbac.Application;

namespace Yi.Framework.Bbs.Application
{
    [DependsOn(typeof(YiFrameworkBbsDomainModule),
        typeof(YiFrameworkBbsApplicationContractsModule),


        typeof(YiFrameworkRbacApplicationModule)
        )]
    public class YiFrameworkBbsApplicationModule : AbpModule
    {

    }
}