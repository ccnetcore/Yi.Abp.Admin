using Yi.Framework.ChatHub.Domain;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.ChatHub.Application
{
    [DependsOn(typeof(YiFrameworkChatHubDomainModule),

        typeof(YiFrameworkDddApplicationContractsModule)
        )]
    public class YiFrameworkChatHubApplicationModule : AbpModule
    {

    }
}