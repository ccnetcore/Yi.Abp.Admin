using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Http.Client;
using Yi.Abp.Tool.HttpApi.Client;

namespace Yi.Abp.Tool
{
    [DependsOn(typeof(YiAbpToolHttpApiClientModule)
        )]
    public class YiAbpToolModule : AbpModule
    {

        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
           // var configuration = context.Services.GetConfiguration();
            Configure<AbpRemoteServiceOptions>(options =>
            {
                options.RemoteServices.Default =
                    new RemoteServiceConfiguration("https://ccnetcore.com:19009");
            });
        }
    }
}
