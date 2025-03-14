using Hangfire;
using Hangfire.MemoryStorage;
using StackExchange.Redis;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.BackgroundWorkers;
using Yi.Abp.Application;
using Yi.Abp.SqlsugarCore;

namespace Yi.Abp.Test
{
    [DependsOn(
        typeof(YiAbpSqlSugarCoreModule),
        typeof(YiAbpApplicationModule),
        
        typeof(AbpAutofacModule)
        )]
    public class YiAbpTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
        }
    }
}
