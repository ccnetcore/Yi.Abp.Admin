using Volo.Abp.Modularity;
using Yi.Abp.HttpApi.Client;

namespace Yi.Abp.Client.WebApi
{
    [DependsOn(typeof(YiAbpHttpApiClientModule))]
    public class YiAbpClientWebApiModule:AbpModule
    {
    }
}
