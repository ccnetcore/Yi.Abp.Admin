using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Data;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Abp.Web.Jobs
{
    public class DemoResetJob  : HangfireBackgroundWorkerBase
    {
        private ISqlSugarDbContext _dbContext;
        private ILogger<DemoResetJob> _logger => LoggerFactory.CreateLogger<DemoResetJob>();
        private IDataSeeder _dataSeeder;
        private IConfiguration _configuration;
        public DemoResetJob(ISqlSugarDbContext dbContext, IDataSeeder dataSeeder, IConfiguration configuration)
        {
            _dbContext = dbContext;
            RecurringJobId = "重置demo环境";
            //每天1点和13点进行重置demo环境
            CronExpression = "0 0 1,13 * * ?";
           
            _dataSeeder = dataSeeder;
            _configuration = configuration;
        }
        
        public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
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
