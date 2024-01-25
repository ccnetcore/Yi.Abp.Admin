using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Yi.Abp.Application;
using Yi.Abp.SqlsugarCore;
using Yi.Framework.Rbac.SqlSugarCore;
using Yi.Framework.SqlSugarCore;

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
