using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Rbac.Application;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Managers;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.SqlSugarCore;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Test
{
    [DependsOn(
        typeof(YiFrameworkRbacApplicationModule),
        typeof(YiFrameworkRbacSqlSugarCoreModule),

        typeof(AbpAutofacModule),
        typeof(AbpAuditingModule)
        )]
    public class YiFrameworkRbacTestModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundWorkerQuartzOptions>(options =>
            {
                options.IsAutoRegisterEnabled = false;
            });
            Configure<AbpBackgroundWorkerOptions> (options =>
            {
                options.IsEnabled = false; //禁用作业执行
            });
            Configure<DbConnOptions>(options =>
            {
                options.Url = $"DataSource=yi-rbac-test-{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.db";

            });
        }

        public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var services = context.ServiceProvider;

            #region 给默认角色设置一些权限，防止注册后无权限禁止登录
            var roleManager = services.GetRequiredService<RoleManager>();
            var roleRep = services.GetRequiredService<ISqlSugarRepository<RoleAggregateRoot>>();
            var menuRep = services.GetRequiredService<ISqlSugarRepository<MenuAggregateRoot>>();
            var defaultRoleEntity = await roleRep._DbQueryable.Where(x => x.RoleCode == UserConst.DefaultRoleCode).FirstAsync();
            var menuIds = await menuRep._DbQueryable.Where(x => x.PermissionCode.Contains("user")).Select(x => x.Id).ToListAsync();
            await roleManager.GiveRoleSetMenuAsync(new List<Guid> { defaultRoleEntity.Id }, menuIds);
            #endregion
        }

    }
}
