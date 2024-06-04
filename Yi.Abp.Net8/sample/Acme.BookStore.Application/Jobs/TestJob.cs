using Quartz;
using SqlSugar;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Acme.BookStore.Application.Jobs
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class TestJob : QuartzBackgroundWorkerBase
    {
        private ISqlSugarRepository<UserAggregateRoot> _repository;
        public TestJob(ISqlSugarRepository<UserAggregateRoot> repository)
        {
            _repository = repository;
            JobDetail = JobBuilder.Create<TestJob>().WithIdentity(nameof(TestJob)).Build();
            Trigger = TriggerBuilder.Create().WithIdentity(nameof(TestJob)).StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInSeconds(1000 * 60)
                .RepeatForever())
            .Build();
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            //定时任务，非常简单
            Console.WriteLine("你好，世界");
            // var eneities= await _repository.GetListAsync();
            //var entities=   await _sqlSugarClient.Queryable<UserEntity>().ToListAsync();
            //await Console.Out.WriteLineAsync(entities.Count().ToString());
        }
    }
}
