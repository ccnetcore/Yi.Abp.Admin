using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;
using Yi.Framework.SqlSugarCore.Uow;

namespace Yi.Framework.SqlSugarCore
{
    [DependsOn(typeof(AbpDddDomainModule))]
    public class YiFrameworkSqlSugarCoreModule : AbpModule
    {
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var service = context.Services;
            var configuration = service.GetConfiguration();
            Configure<DbConnOptions>(configuration.GetSection("DbConnOptions"));

            service.TryAddScoped<ISqlSugarDbContext, SqlSugarDbContext>();

            //不开放sqlsugarClient
            //service.AddTransient<ISqlSugarClient>(x => x.GetRequiredService<ISqlsugarDbContext>().SqlSugarClient);


            service.AddTransient(typeof(IRepository<>), typeof(SqlSugarRepository<>));
            service.AddTransient(typeof(IRepository<,>), typeof(SqlSugarRepository<,>));
            service.AddTransient(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
            service.AddTransient(typeof(ISqlSugarRepository<,>), typeof(SqlSugarRepository<,>));

            service.AddTransient(typeof(ISugarDbContextProvider<>), typeof(UnitOfWorkSqlsugarDbContextProvider<>));


            return Task.CompletedTask;
        }


        public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            //进行CodeFirst
            var service = context.ServiceProvider;
            var options = service.GetRequiredService<IOptions<DbConnOptions>>().Value;
            
           var _logger= service.GetRequiredService<ILogger<YiFrameworkSqlSugarCoreModule>>();


            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("==========Yi-SQL配置:==========");
            sb.AppendLine($"数据库连接字符串：{options.Url}");
            sb.AppendLine($"数据库类型：{options.DbType.ToString()}");
            sb.AppendLine($"是否开启种子数据：{options.EnabledDbSeed}");
            sb.AppendLine($"是否开启CodeFirst：{options.EnabledCodeFirst}");
            sb.AppendLine($"是否开启Saas多租户：{options.EnabledSaasMultiTenancy}");
            sb.AppendLine("===============================");


            _logger.LogInformation(sb.ToString());
            //Todo：准备支持多租户种子数据及CodeFirst

            if (options.EnabledCodeFirst)
            {
                CodeFirst(service);
            }
            if (options.EnabledDbSeed)
            {
                await DataSeedAsync(service);
            }
        }

        private void CodeFirst(IServiceProvider service)
        {

            var moduleContainer = service.GetRequiredService<IModuleContainer>();
            var db = service.GetRequiredService<ISqlSugarDbContext>().SqlSugarClient;

            //尝试创建数据库
            db.DbMaintenance.CreateDatabase();

            List<Type> types = new List<Type>();
            foreach (var module in moduleContainer.Modules)
            {
                types.AddRange(module.Assembly.GetTypes()
                    .Where(x => x.GetCustomAttribute<IgnoreCodeFirstAttribute>() == null)
                    .Where(x => x.GetCustomAttribute<SugarTable>() != null)
                    .Where(x => x.GetCustomAttribute<SplitTableAttribute>() is null));
            }
            if (types.Count > 0)
            {
                db.CopyNew().CodeFirst.InitTables(types.ToArray());
            }

        }

        private async Task DataSeedAsync(IServiceProvider service)
        {
            var dataSeeder = service.GetRequiredService<IDataSeeder>();
            await dataSeeder.SeedAsync();
        }
    }
}
