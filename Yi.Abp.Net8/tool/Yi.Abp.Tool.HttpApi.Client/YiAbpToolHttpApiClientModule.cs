using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Http.Client;
using Yi.Abp.Tool.Application.Contracts;

namespace Yi.Abp.Tool.HttpApi.Client
{
    [DependsOn(typeof(AbpHttpClientModule),
            typeof(AbpAutofacModule),
            typeof(YiAbpToolApplicationContractsModule))]
    public class YiAbpToolHttpApiClientModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //创建动态客户端代理
            context.Services.AddHttpClientProxies(
                typeof(YiAbpToolApplicationContractsModule).Assembly

            );
            Configure<AbpRemoteServiceOptions>(options =>
            {
                options.RemoteServices.Default =
                    new RemoteServiceConfiguration("http://localhost:19002");
            });
        }
    }
}
