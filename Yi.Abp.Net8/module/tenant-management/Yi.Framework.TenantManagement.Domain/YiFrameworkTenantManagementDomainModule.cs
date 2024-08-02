using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;

namespace Yi.Framework.TenantManagement.Domain
{
    [DependsOn(typeof(AbpDddDomainModule),
        typeof(AbpTenantManagementDomainSharedModule))]
    public class YiFrameworkTenantManagementDomainModule : AbpModule
    {

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;
            services.Replace(new ServiceDescriptor(typeof(ITenantStore), typeof(SqlSugarAndConfigurationTenantStore), ServiceLifetime.Transient));

            services.Replace(new ServiceDescriptor(typeof(IConnectionStringResolver), typeof(YiMultiTenantConnectionStringResolver), ServiceLifetime.Transient));
        }
    }
}
