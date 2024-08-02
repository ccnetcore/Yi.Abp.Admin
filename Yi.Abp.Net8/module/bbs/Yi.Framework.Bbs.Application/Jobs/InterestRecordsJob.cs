using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;
using Yi.Framework.Bbs.Domain.Managers;

namespace Yi.Framework.Bbs.Application.Jobs
{
    public class InterestRecordsJob : QuartzBackgroundWorkerBase
    {
        private BankManager _bankManager;
        public InterestRecordsJob(BankManager bankManager)
        {
            _bankManager = bankManager;
            JobDetail = JobBuilder.Create<InterestRecordsJob>().WithIdentity(nameof(InterestRecordsJob)).Build();

            //每个小时整点执行一次

            Trigger = TriggerBuilder.Create().WithIdentity(nameof(InterestRecordsJob)).WithCronSchedule("0 0 * * * ?").Build();

            //测试
            //            Trigger = TriggerBuilder.Create().WithIdentity(nameof(InterestRecordsJob))
            //.WithSimpleSchedule(x => x
            //    .WithIntervalInSeconds(10)
            //    .RepeatForever())
            //.Build();
        }
        public override async Task Execute(IJobExecutionContext context)
        {
            //创建一个记录，莫得了
            await _bankManager.GetCurrentInterestRate();
        }
    }
}
