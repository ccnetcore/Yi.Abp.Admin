using Yi.Framework.ChatHub.Domain;
using Yi.Framework.SqlSugarCore;

namespace Yi.Framework.ChatHub.SqlSugarCore
{
    [DependsOn(
        typeof(YiFrameworkChatHubDomainModule),

        typeof(YiFrameworkSqlSugarCoreModule))]
    public class YiFrameworkChatHubSqlSugarCoreModule : AbpModule
    {

    }
}