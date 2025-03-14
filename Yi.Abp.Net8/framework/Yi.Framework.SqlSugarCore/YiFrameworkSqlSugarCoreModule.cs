using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.MultiTenancy.ConfigurationStore;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;
using Yi.Framework.SqlSugarCore.Uow;

namespace Yi.Framework.SqlSugarCore
{
    /// <summary>
    /// SqlSugar Core模块
    /// </summary>
    [DependsOn(typeof(AbpDddDomainModule))]
    public class YiFrameworkSqlSugarCoreModule : AbpModule
    {
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var services = context.Services;
            var configuration = services.GetConfiguration();

            // 配置数据库连接选项
            ConfigureDbOptions(services, configuration);

            // 配置GUID生成器
            ConfigureGuidGenerator(services);

            // 注册仓储和服务
            RegisterRepositories(services);

            return Task.CompletedTask;
        }

        private void ConfigureDbOptions(IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("DbConnOptions");
            Configure<DbConnOptions>(section);

            var dbConnOptions = new DbConnOptions();
            section.Bind(dbConnOptions);

            // 配置默认连接字符串
            Configure<AbpDbConnectionOptions>(options => 
            { 
                options.ConnectionStrings.Default = dbConnOptions.Url; 
            });

            // 配置默认租户
            ConfigureDefaultTenant(services, dbConnOptions);
        }

        private void ConfigureGuidGenerator(IServiceCollection services)
        {
            var dbConnOptions = services.GetConfiguration()
                .GetSection("DbConnOptions")
                .Get<DbConnOptions>();

            var guidType = GetSequentialGuidType(dbConnOptions?.DbType);
            Configure<AbpSequentialGuidGeneratorOptions>(options =>
            {
                options.DefaultSequentialGuidType = guidType;
            });
        }

        private void RegisterRepositories(IServiceCollection services)
        {
            services.TryAddTransient<ISqlSugarDbContext, SqlSugarDbContextFactory>();
            services.AddTransient(typeof(IRepository<>), typeof(SqlSugarRepository<>));
            services.AddTransient(typeof(IRepository<,>), typeof(SqlSugarRepository<,>));
            services.AddTransient(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
            services.AddTransient(typeof(ISqlSugarRepository<,>), typeof(SqlSugarRepository<,>));
            services.AddTransient(typeof(ISugarDbContextProvider<>), typeof(UnitOfWorkSqlsugarDbContextProvider<>));
            services.AddSingleton<ISerializeService, SqlSugarNonPublicSerializer>();
            services.AddYiDbContext<DefaultSqlSugarDbContext>();
        }

        private void ConfigureDefaultTenant(IServiceCollection services, DbConnOptions dbConfig)
        {
            Configure<AbpDefaultTenantStoreOptions>(options => 
            {
                var tenants = options.Tenants.ToList();
                
                // 规范化租户名称
                foreach (var tenant in tenants)
                {
                    tenant.NormalizedName = tenant.Name.Contains("@") 
                        ? tenant.Name.Substring(0, tenant.Name.LastIndexOf("@")) 
                        : tenant.Name;
                }

                // 添加默认租户
                tenants.Insert(0, new TenantConfiguration
                {
                    Id = Guid.Empty,
                    Name = ConnectionStrings.DefaultConnectionStringName,
                    NormalizedName = ConnectionStrings.DefaultConnectionStringName,
                    ConnectionStrings = new ConnectionStrings 
                    { 
                        { ConnectionStrings.DefaultConnectionStringName, dbConfig.Url } 
                    },
                    IsActive = true
                });

                options.Tenants = tenants.ToArray();
            });
        }

        private SequentialGuidType GetSequentialGuidType(DbType? dbType)
        {
            return dbType switch
            {
                DbType.MySql or DbType.PostgreSQL => SequentialGuidType.SequentialAsString,
                DbType.SqlServer => SequentialGuidType.SequentialAtEnd,
                DbType.Oracle => SequentialGuidType.SequentialAsBinary,
                _ => SequentialGuidType.SequentialAtEnd
            };
        }

        public override async Task OnPreApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var serviceProvider = context.ServiceProvider;
            var options = serviceProvider.GetRequiredService<IOptions<DbConnOptions>>().Value;
            var logger = serviceProvider.GetRequiredService<ILogger<YiFrameworkSqlSugarCoreModule>>();

            // 记录配置信息
            LogConfiguration(logger, options);

            // 初始化数据库
            if (options.EnabledCodeFirst)
            {
                await InitializeDatabase(serviceProvider);
            }

            // 初始化种子数据
            if (options.EnabledDbSeed)
            {
                await InitializeSeedData(serviceProvider);
            }
        }

        private void LogConfiguration(ILogger logger, DbConnOptions options)
        {
            var logMessage = new StringBuilder()
                .AppendLine()
                .AppendLine("==========Yi-SQL配置:==========")
                .AppendLine($"数据库连接字符串：{options.Url}")
                .AppendLine($"数据库类型：{options.DbType}")
                .AppendLine($"是否开启种子数据：{options.EnabledDbSeed}")
                .AppendLine($"是否开启CodeFirst：{options.EnabledCodeFirst}")
                .AppendLine($"是否开启Saas多租户：{options.EnabledSaasMultiTenancy}")
                .AppendLine("===============================")
                .ToString();

            logger.LogInformation(logMessage);
        }

        private async Task InitializeDatabase(IServiceProvider serviceProvider)
        {
            var moduleContainer = serviceProvider.GetRequiredService<IModuleContainer>();
            var db = serviceProvider.GetRequiredService<ISqlSugarDbContext>().SqlSugarClient;

            // 创建数据库
            db.DbMaintenance.CreateDatabase();

            // 获取需要创建表的实体类型
            var entityTypes = moduleContainer.Modules
                .SelectMany(m => m.Assembly.GetTypes())
                .Where(t => t.GetCustomAttribute<IgnoreCodeFirstAttribute>() == null
                    && t.GetCustomAttribute<SugarTable>() != null
                    && t.GetCustomAttribute<SplitTableAttribute>() == null)
                .ToList();

            if (entityTypes.Any())
            {
                db.CopyNew().CodeFirst.InitTables(entityTypes.ToArray());
            }
        }

        private async Task InitializeSeedData(IServiceProvider serviceProvider)
        {
            var dataSeeder = serviceProvider.GetRequiredService<IDataSeeder>();
            await dataSeeder.SeedAsync();
        }
    }
}