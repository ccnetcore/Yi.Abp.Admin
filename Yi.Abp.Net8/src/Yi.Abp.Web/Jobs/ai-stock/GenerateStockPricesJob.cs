using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Yi.Framework.Stock.Domain.Managers;

namespace Yi.Abp.Web.Jobs.ai_stock
{
    public class GenerateStockPricesJob : HangfireBackgroundWorkerBase
    {
        private readonly StockMarketManager _stockMarketManager;
        
        public GenerateStockPricesJob(StockMarketManager stockMarketManager)
        {
            _stockMarketManager = stockMarketManager;
            
            RecurringJobId = "AI股票价格生成";
            //每天凌晨1点执行一次
            CronExpression = "0 0 1 * * ?";
        }

        public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await _stockMarketManager.GenerateStocksAsync();
        }
    }
} 