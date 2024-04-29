using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Yi.Abp.Application;
using Yi.Abp.SqlsugarCore;

namespace Yi.Abp.Test
{
    [DependsOn(
        typeof(YiAbpSqlSugarCoreModule),
        typeof(YiAbpApplicationModule),

        typeof(AbpAutofacModule),
        typeof(AbpAuditingModule)
        )]
    public class YiAbpTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}
