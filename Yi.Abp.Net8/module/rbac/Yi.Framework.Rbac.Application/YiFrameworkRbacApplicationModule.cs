using Lazy.Captcha.Core.Generator;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Modularity;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts;
using Yi.Framework.Rbac.Application.Jobs;
using Yi.Framework.Rbac.Domain;

namespace Yi.Framework.Rbac.Application
{
    [DependsOn(
        typeof(YiFrameworkRbacApplicationContractsModule),
        typeof(YiFrameworkRbacDomainModule),


        typeof(YiFrameworkDddApplicationModule)
        )]
    public class YiFrameworkRbacApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var service = context.Services;

            service.AddCaptcha(options =>
            {
                options.CaptchaType = CaptchaType.ARITHMETIC;
            });
        }

        public async override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
        }
    }
}
