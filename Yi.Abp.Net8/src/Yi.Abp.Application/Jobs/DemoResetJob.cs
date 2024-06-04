using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Logging;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Data;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Application.Jobs
{
    public class DemoResetJob : QuartzBackgroundWorkerBase
    {
        private ISqlSugarDbContext _dbContext;
        private ILogger<DemoResetJob> _logger => LoggerFactory.CreateLogger<DemoResetJob>();
        private IDataSeeder _dataSeeder;
        private IConfiguration _configuration;
        public DemoResetJob(ISqlSugarDbContext dbContext, IDataSeeder dataSeeder, IConfiguration configuration)
        {
            _dbContext = dbContext;
            JobDetail = JobBuilder.Create<DemoResetJob>().WithIdentity(nameof(DemoResetJob)).Build();

            //每天01点与13点,演示环境进行重置
            Trigger = TriggerBuilder.Create().WithIdentity(nameof(DemoResetJob)).WithCronSchedule("0 0 1,13 * * ? ").Build();
           // Trigger = TriggerBuilder.Create().WithIdentity(nameof(DemoResetJob)).WithSimpleSchedule(x=>x.WithIntervalInSeconds(10)).Build();
            _dataSeeder = dataSeeder;
            _configuration = configuration;
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            //开启演示环境重置功能
            if (_configuration.GetSection("EnableDemoReset").Get<bool>())
            {
                //定时任务，非常简单
                _logger.LogWarning("演示环境正在还原！");
                var db = _dbContext.SqlSugarClient.CopyNew();
                db.DbMaintenance.TruncateTable<UserAggregateRoot>();
                db.DbMaintenance.TruncateTable<UserRoleEntity>();
                db.DbMaintenance.TruncateTable<RoleAggregateRoot>();
                db.DbMaintenance.TruncateTable<RoleMenuEntity>();
                db.DbMaintenance.TruncateTable<MenuAggregateRoot>();
                db.DbMaintenance.TruncateTable<RoleDeptEntity>();
                db.DbMaintenance.TruncateTable<DeptAggregateRoot>();
                db.DbMaintenance.TruncateTable<PostAggregateRoot>();
                db.DbMaintenance.TruncateTable<UserPostEntity>();

                //删除种子数据
                await _dataSeeder.SeedAsync();
                _logger.LogWarning("演示环境还原成功！");

            }


        }
    }
}
