using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.Stock.Domain.Managers;

namespace Yi.Abp.Web.Jobs.ai_stock
{
    public class GenerateNewsJob : HangfireBackgroundWorkerBase
    {
        private NewsManager _newsManager;
        
        public GenerateNewsJob(NewsManager newsManager)
        {
            _newsManager = newsManager;
            
            RecurringJobId = "AI股票新闻生成";
            //每个小时整点执行一次
            CronExpression = "0 0 * * * ?";
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            // 每次触发只有2/24的概率执行生成新闻
            var random = new Random();
            var probability = random.Next(0, 24);
            
            if (probability < 2)
            {
                await _newsManager.GenerateNewsAsync();
            }
        }
    }
}