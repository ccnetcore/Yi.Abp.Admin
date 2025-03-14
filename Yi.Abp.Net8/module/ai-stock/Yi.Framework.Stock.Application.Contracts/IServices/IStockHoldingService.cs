using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockHolding;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockTransaction;

namespace Yi.Framework.Stock.Application.Contracts.IServices
{
    /// <summary>
    /// 用户持仓服务接口
    /// </summary>
    public interface IStockHoldingService : IApplicationService
    {
        /// <summary>
        /// 获取当前用户的持仓列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>持仓列表</returns>
        Task<PagedResultDto<StockHoldingDto>> GetUserHoldingsAsync(StockHoldingGetListInputDto input);
        
        /// <summary>
        /// 获取当前用户的交易记录
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>交易记录列表</returns>
        Task<PagedResultDto<StockTransactionDto>> GetUserTransactionsAsync(StockTransactionGetListInputDto input);
    }
} 