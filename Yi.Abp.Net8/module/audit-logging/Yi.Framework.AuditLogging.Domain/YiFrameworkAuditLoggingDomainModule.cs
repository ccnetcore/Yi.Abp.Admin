using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Auditing;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Framework.AuditLogging.Domain.Shared;

namespace Yi.Framework.AuditLogging.Domain
{
    [DependsOn(typeof(YiFrameworkAuditLoggingDomainSharedModule),
        
        
        typeof(AbpDddDomainModule),
        typeof(AbpAuditingModule)
        )]
    public class YiFrameworkAuditLoggingDomainModule:AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}
