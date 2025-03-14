using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockPrice;

namespace Yi.Framework.Stock.Application.Contracts.IServices
{
    /// <summary>
    /// 股市服务接口
    /// </summary>
    public interface IStockMarketService : IApplicationService
    {
        /// <summary>
        /// 获取股市列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>股市列表</returns>
        Task<PagedResultDto<StockMarketDto>> GetStockMarketListAsync(StockMarketGetListInputDto input);

        /// <summary>
        /// 获取股市价格记录看板
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>股价记录列表</returns>
        Task<PagedResultDto<StockPriceRecordDto>> GetStockPriceRecordListAsync(StockPriceRecordGetListInputDto input);

        /// <summary>
        /// 买入股票
        /// </summary>
        /// <param name="input">买入股票参数</param>
        /// <returns>操作结果</returns>
        Task BuyStockAsync(BuyStockInputDto input);

        /// <summary>
        /// 卖出股票
        /// </summary>
        /// <param name="input">卖出股票参数</param>
        /// <returns>操作结果</returns>
        Task SellStockAsync(SellStockInputDto input);

        /// <summary>
        /// 生成最新股票记录
        /// </summary>
        /// <returns>操作结果</returns>
        Task GenerateStocksAsync();
    }
} 