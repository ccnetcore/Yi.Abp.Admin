using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.Bbs.Domain.Managers;

namespace Yi.Framework.Bbs.Application.Jobs
{
    public class InterestRecordsJob : HangfireBackgroundWorkerBase
    {
        private BankManager _bankManager;
        public InterestRecordsJob(BankManager bankManager)
        {
            _bankManager = bankManager;
            
            RecurringJobId = "银行利息积分刷新";
            //每个小时整点执行一次
            CronExpression = "0 0 * * * ?";
            
            // JobDetail = JobBuilder.Create<InterestRecordsJob>().WithIdentity(nameof(InterestRecordsJob)).Build();
            //
            // //每个小时整点执行一次
            //
            // Trigger = TriggerBuilder.Create().WithIdentity(nameof(InterestRecordsJob)).WithCronSchedule("0 0 * * * ?").Build();

            //测试
            //            Trigger = TriggerBuilder.Create().WithIdentity(nameof(InterestRecordsJob))
            //.WithSimpleSchedule(x => x
            //    .WithIntervalInSeconds(10)
            //    .RepeatForever())
            //.Build();
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //创建一个记录，莫得了
            await _bankManager.GetCurrentInterestRate();
        }
    }
}
