using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.Rbac.SqlSugarCore.Repositories;

namespace Yi.Abp.Test
{
    public class YiAbpTestBase :AbpTestBaseWithServiceProvider
    {
        public ILogger Logger { get; private set; }
        protected IServiceScope TestServiceScope { get; }
        public YiAbpTestBase()
        {
            IHost host = Host.CreateDefaultBuilder()
               .UseAutofac()
               .ConfigureServices((host, service) =>
               {
                   ConfigureServices(host, service);
                   service.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());
                   /*application= */
                   service.AddApplicationAsync<YiAbpTestModule>().Wait();
               })
               .ConfigureAppConfiguration(this.ConfigureAppConfiguration)
               .Build();

            this.ServiceProvider = host.Services;
            this.TestServiceScope = ServiceProvider.CreateScope();
            this.Logger = (ILogger)this.ServiceProvider.GetRequiredService(typeof(ILogger<>).MakeGenericType(this.GetType()));

            host.InitializeAsync().Wait();
        }


        public virtual void ConfigureServices(HostBuilderContext host, IServiceCollection service)
        { 
        }
        protected virtual void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddJsonFile("appsettings.json");
            configurationBuilder.AddJsonFile("appsettings.Development.json");
            
        }
    }
}
