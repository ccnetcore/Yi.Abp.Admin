using Yi.Framework.ChatHub.Domain.Shared;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.ChatHub.Application.Contracts
{
    [DependsOn(typeof(YiFrameworkChatHubDomainSharedModule),

        typeof(YiFrameworkDddApplicationContractsModule)
        )]
    public class YiFrameworkChatHubApplicationContractsModule : AbpModule
    {

    }
}