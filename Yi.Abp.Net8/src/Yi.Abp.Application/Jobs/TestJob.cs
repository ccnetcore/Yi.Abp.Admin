using Quartz;
using Volo.Abp.BackgroundWorkers.Quartz;

namespace Yi.Framework.Rbac.Application.Jobs
{
    public class TestJob : QuartzBackgroundWorkerBase
    {
        public TestJob()
        {
            JobDetail = JobBuilder.Create<TestJob>().WithIdentity(nameof(TestJob)).Build();
            Trigger = TriggerBuilder.Create().WithIdentity(nameof(TestJob)).WithCronSchedule("* * * * * ? *").Build();
        }
        public override Task Execute(IJobExecutionContext context)
        {
            //定时任务，非常简单
            //Console.WriteLine("你好，世界");
            return Task.CompletedTask;
        }
    }
}
