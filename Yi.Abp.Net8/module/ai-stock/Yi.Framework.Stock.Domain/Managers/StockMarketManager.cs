using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.Stock.Domain.Shared;
using Yi.Framework.Stock.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel;
using Yi.Framework.Stock.Domain.Managers.SemanticKernel.Plugins;
using Microsoft.Extensions.Hosting;
using System.Text;
using System.IO;

namespace Yi.Framework.Stock.Domain.Managers
{
    /// <summary>
    /// 股市领域服务
    /// </summary>
    /// <remarks>
    /// 处理股票交易相关业务，例如买入、卖出等
    /// </remarks>
    public class StockMarketManager : DomainService
    {
        private readonly ISqlSugarRepository<StockHoldingAggregateRoot> _stockHoldingRepository;
        private readonly ISqlSugarRepository<StockTransactionEntity> _stockTransactionRepository;
        private readonly ISqlSugarRepository<StockPriceRecordEntity> _stockPriceRecordRepository;
        private readonly ISqlSugarRepository<StockMarketAggregateRoot> _stockMarketRepository;
        private readonly ILocalEventBus _localEventBus;
        private readonly SemanticKernelClient _skClient;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly NewsManager _newsManager;

        public StockMarketManager(
            ISqlSugarRepository<StockHoldingAggregateRoot> stockHoldingRepository,
            ISqlSugarRepository<StockTransactionEntity> stockTransactionRepository,
            ISqlSugarRepository<StockPriceRecordEntity> stockPriceRecordRepository,
            ISqlSugarRepository<StockMarketAggregateRoot> stockMarketRepository,
            ILocalEventBus localEventBus, 
            SemanticKernelClient skClient,
            IHostEnvironment hostEnvironment,
            NewsManager newsManager)
        {
            _stockHoldingRepository = stockHoldingRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _stockPriceRecordRepository = stockPriceRecordRepository;
            _stockMarketRepository = stockMarketRepository;
            _localEventBus = localEventBus;
            _skClient = skClient;
            _hostEnvironment = hostEnvironment;
            _newsManager = newsManager;
        }

        /// <summary>
        /// 购买股票
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="stockId">股票ID</param>
        /// <param name="quantity">购买数量</param>
        /// <returns></returns>
        public async Task BuyStockAsync(Guid userId, Guid stockId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new UserFriendlyException("购买数量必须大于0");
            }

            // 通过stockId查询获取股票信息
            var stockInfo = await _stockMarketRepository.GetFirstAsync(s => s.Id == stockId);
            if (stockInfo == null)
            {
                throw new UserFriendlyException("找不到指定的股票");
            }

            string stockCode = stockInfo.MarketCode; // 根据实际字段调整
            string stockName = stockInfo.MarketName; // 根据实际字段调整

            // 获取当前股票价格
            decimal currentPrice = await GetCurrentStockPriceAsync(stockId);
            
            // 计算总金额和手续费
            decimal totalAmount = currentPrice * quantity;
            decimal fee = CalculateTradingFee(totalAmount, TransactionTypeEnum.Buy);
            decimal totalCost = totalAmount + fee;

            // 扣减用户资金
            await _localEventBus.PublishAsync(
                new MoneyChangeEventArgs { UserId = userId, Number = -totalCost }, false);

            // 更新或创建用户持仓
            var holding = await _stockHoldingRepository.GetFirstAsync(h => 
                h.UserId == userId && 
                h.StockId == stockId && 
                !h.IsDeleted);

            if (holding == null)
            {
                // 创建新持仓
                holding = new StockHoldingAggregateRoot(
                    userId,
                    stockId,
                    stockCode,
                    stockName,
                    quantity,
                    currentPrice);

                await _stockHoldingRepository.InsertAsync(holding);
            }
            else
            {
                // 更新现有持仓
                holding.AddQuantity(quantity, currentPrice);
                await _stockHoldingRepository.UpdateAsync(holding);
            }
            // 发布交易事件
            await _localEventBus.PublishAsync(new StockTransactionEto
            {
                UserId = userId,
                StockId = stockId,
                StockCode = stockCode,
                StockName = stockName,
                TransactionType = TransactionTypeEnum.Buy,
                Price = currentPrice,
                Quantity = quantity,
                TotalAmount = totalAmount,
                Fee = fee
            }, false);
        }

        /// <summary>
        /// 卖出股票
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="stockId">股票ID</param>
        /// <param name="quantity">卖出数量</param>
        /// <returns></returns>
        public async Task SellStockAsync(Guid userId, Guid stockId, int quantity)
        {
            // 验证卖出时间
            VerifySellTime();

            if (quantity <= 0)
            {
                throw new UserFriendlyException("卖出数量必须大于0");
            }

            // 获取用户持仓
            var holding = await _stockHoldingRepository.GetFirstAsync(h => 
                h.UserId == userId && 
                h.StockId == stockId && 
                !h.IsDeleted);

            if (holding == null)
            {
                throw new UserFriendlyException("您没有持有该股票");
            }

            if (holding.Quantity < quantity)
            {
                throw new UserFriendlyException("持仓数量不足");
            }

            // 获取当前股票价格
            decimal currentPrice = await GetCurrentStockPriceAsync(stockId);
            
            // 计算总金额和手续费
            decimal totalAmount = currentPrice * quantity;
            decimal fee = CalculateTradingFee(totalAmount, TransactionTypeEnum.Sell);
            decimal actualIncome = totalAmount - fee;

            // 增加用户资金
            await _localEventBus.PublishAsync(
                new MoneyChangeEventArgs { UserId = userId, Number = actualIncome }, false);

            // 更新用户持仓
            holding.ReduceQuantity(quantity);
            
            if (holding.Quantity > 0)
            {
                await _stockHoldingRepository.UpdateAsync(holding);
            }
            else
            {
                await _stockHoldingRepository.DeleteAsync(holding);
            }

            // 发布交易事件
            await _localEventBus.PublishAsync(new StockTransactionEto
            {
                UserId = userId,
                StockId = stockId,
                StockCode = holding.StockCode,
                StockName = holding.StockName,
                TransactionType = TransactionTypeEnum.Sell,
                Price = currentPrice,
                Quantity = quantity,
                TotalAmount = totalAmount,
                Fee = fee
            }, false);
        }

        /// <summary>
        /// 获取股票当前价格
        /// </summary>
        /// <param name="stockId">股票ID</param>
        /// <returns>当前价格</returns>
        public async Task<decimal> GetCurrentStockPriceAsync(Guid stockId)
        {
            // 获取最新的价格记录
            var latestPriceRecord = await _stockPriceRecordRepository._DbQueryable
                .Where(p => p.StockId == stockId)
                .Where(x=>x.RecordTime<=DateTime.Now)
                .OrderByDescending(p => p.RecordTime)
                .FirstAsync();

            if (latestPriceRecord == null)
            {
                throw new UserFriendlyException("无法获取股票价格信息");
            }

            return latestPriceRecord.CurrentPrice;
        }

        /// <summary>
        /// 计算交易手续费
        /// </summary>
        /// <param name="amount">交易金额</param>
        /// <param name="transactionType">交易类型</param>
        /// <returns>手续费</returns>
        private decimal CalculateTradingFee(decimal amount, TransactionTypeEnum transactionType)
        {
            // 买入不收手续费，卖出收2%
            decimal feeRate = transactionType == TransactionTypeEnum.Buy ? 0m : 0.02m;
            return amount * feeRate;
        }

        /// <summary>
        /// 验证卖出时间
        /// </summary>
        /// <exception cref="UserFriendlyException">如果不在允许卖出的时间范围内</exception>
        private void VerifySellTime()
        {
            // 如果是开发环境，跳过验证
            if (_hostEnvironment.IsDevelopment())
            {
                return;
            }

            DateTime now = DateTime.Now;
            
            // 检查是否为工作日（周一到周五）
            if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new UserFriendlyException("股票只能在工作日（周一至周五）卖出");
            }

            // 检查是否在下午5点到6点之间
            if (now.Hour < 17 || now.Hour >= 18)
            {
                throw new UserFriendlyException("股票只能在下午5点到6点之间卖出");
            }
        }
        
        
        /// <summary>
        /// 批量保存多个股票的最新价格记录
        /// </summary>
        /// <param name="priceRecords">价格记录列表</param>
        /// <returns>保存的记录数量</returns>
        public async Task BatchSaveStockPriceRecordsAsync(List<StockPriceRecordEntity> priceRecords)
        {
            if (priceRecords == null || !priceRecords.Any())
            {
                return;
            }
            
            // 验证数据
            for (int i = 0; i < priceRecords.Count; i++)
            {
                var record = priceRecords[i];
                if (record.CurrentPrice <= 0)
                {
                    throw new UserFriendlyException($"股票ID {record.StockId} 的价格必须大于0");
                }
                
                // 计算交易额（如果未设置）
                if (record.Turnover == 0 && record.Volume > 0)
                {
                    record.Turnover = record.CurrentPrice * record.Volume;
                }
            }
            
            await _stockPriceRecordRepository.InsertManyAsync(priceRecords);
        }
        
        public async Task SaveStockAsync(List<StockModel> stockModels)
        {
            if (stockModels == null || !stockModels.Any())
            {
                return;
            }
            
            // 收集所有股票ID
            var stockIds = stockModels.Select(m => m.Id).ToList();
            
            // 一次性查询所有相关股票信息
            var stockMarkets = await _stockMarketRepository.GetListAsync(s => stockIds.Contains(s.Id));
            
            // 构建字典以便快速查找
            var stockMarketsDict = stockMarkets.ToDictionary(s => s.Id);
            
            // 将StockModel转换为StockPriceRecordEntity
            var priceRecords = new List<StockPriceRecordEntity>();
            
            // 获取当前小时的起始时间点
            var currentHour = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
            
            foreach (var stockModel in stockModels)
            {
                if (stockModel.Values == null || !stockModel.Values.Any())
                {
                    continue;
                }
                
                // 从字典中查找股票信息，而不是每次查询数据库
                if (!stockMarketsDict.TryGetValue(stockModel.Id, out var stockMarket))
                {
                    continue;
                }
                
                // 为每个价格点创建一个记录，并设置递增的时间
                for (int i = 0; i < stockModel.Values.Count(); i++)
                {
                    var priceValue = stockModel.Values[i];
                    var recordTime = currentHour.AddHours(i); // 从当前小时开始，每个价格点加1小时
                    
                    var priceRecord = new StockPriceRecordEntity
                    {
                        StockId = stockMarket.Id,
                        CurrentPrice = priceValue,
                        Volume = 0, // 可以根据实际情况设置
                        Turnover = 0, // 可以根据实际情况设置
                        PeriodType = PeriodTypeEnum.Hour,
                        RecordTime = recordTime // 直接在这里设置时间
                    };
                    
                    priceRecords.Add(priceRecord);
                }
            }
            
            // 批量保存价格记录
            if (priceRecords.Any())
            {
                await _stockPriceRecordRepository.InsertManyAsync(priceRecords);
            }
        }

        /// <summary>
        /// 获取所有活跃股票的最新价格
        /// </summary>
        /// <returns>股票ID和最新价格的字典</returns>
        private async Task<Dictionary<Guid, decimal>> GetLatestStockPricesAsync()
        {
            // 获取所有活跃的股票
            var activeStocks = await _stockMarketRepository.GetListAsync(s => s.State && !s.IsDeleted);
            var result = new Dictionary<Guid, decimal>();
            
            foreach (var stock in activeStocks)
            {
                try
                {
                    var price = await GetCurrentStockPriceAsync(stock.Id);
                    result.Add(stock.Id, price);
                }
                catch
                {
                    // 如果获取价格失败，使用默认价格10
                    result.Add(stock.Id, 10m);
                }
            }
            
            return result;
        }

        /// <summary>
        /// 生成最新股票记录
        /// </summary>
        /// <returns></returns>
        public async Task GenerateStocksAsync()
        {
            // 获取所有活跃股票的最新价格
            var stockPrices = await GetLatestStockPricesAsync();
            if (!stockPrices.Any())
            {
                return; // 没有股票数据，直接返回
            }
            
            // 获取所有活跃股票信息，用于构建提示词
            var activeStocks = await _stockMarketRepository.GetListAsync(s => 
                s.State && !s.IsDeleted && stockPrices.Keys.Contains(s.Id));
            
            // 获取最近10条新闻
            var recentNews = await _newsManager.GetRecentNewsAsync(10);
            
            // 构建新闻上下文
            var newsContext = new StringBuilder();
            if (recentNews.Any())
            {
                newsContext.AppendLine("以下是最近的新闻摘要：");
                foreach (var news in recentNews)
                {
                    newsContext.AppendLine($"- {news.CreationTime:yyyy-MM-dd}: {news.Title}");
                    newsContext.AppendLine($"  {news.Summary}");
                    newsContext.AppendLine();
                }
            }
            else
            {
                newsContext.AppendLine("最近没有重要新闻报道。");
            }
            
            // 构建股票信息上下文
            var stocksContext = new StringBuilder();
            stocksContext.AppendLine("以下是需要预测的股票信息：");
            foreach (var stock in activeStocks)
            {
                if (stockPrices.TryGetValue(stock.Id, out var price))
                {
                    stocksContext.AppendLine($"{stock.MarketName}：id:{stock.Id}，简介：{stock.Description} 最后一次价格：{price:F2}");
                }
            }
            
            // 从文件读取问题模板
            string promptTemplate = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "stock", "marketPrompt.txt"));

            // 替换变量
            string question = promptTemplate
                .Replace("{{newsContext}}", newsContext.ToString())
                .Replace("{{stocksContext}}", stocksContext.ToString());
            
            await _skClient.ChatCompletionAsync(question, ("StockPlugins", "save_stocks"));
        }
    }
} 