using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockHolding;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockTransaction;
using Yi.Framework.Stock.Application.Contracts.IServices;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;
using Volo.Abp.Users;

namespace Yi.Framework.Stock.Application.Services
{
    /// <summary>
    /// 用户持仓服务实现
    /// </summary>
    [Authorize]
    public class StockHoldingService : ApplicationService, IStockHoldingService
    {
        private readonly ISqlSugarRepository<StockHoldingAggregateRoot> _stockHoldingRepository;
        private readonly ISqlSugarRepository<StockTransactionEntity> _stockTransactionRepository;
        
        public StockHoldingService(
            ISqlSugarRepository<StockHoldingAggregateRoot> stockHoldingRepository,
            ISqlSugarRepository<StockTransactionEntity> stockTransactionRepository)
        {
            _stockHoldingRepository = stockHoldingRepository;
            _stockTransactionRepository = stockTransactionRepository;
        }
        
        /// <summary>
        /// 获取当前用户的持仓列表
        /// </summary>
        [Authorize]
        [HttpGet("stock/user-holdings")]
        public async Task<PagedResultDto<StockHoldingDto>> GetUserHoldingsAsync(StockHoldingGetListInputDto input)
        {
            Guid userId = CurrentUser.GetId();
            RefAsync<int> total = 0;
            
            var query = _stockHoldingRepository._DbQueryable
                .Where(h => h.UserId == userId)
                .WhereIF(!string.IsNullOrEmpty(input.StockCode), h => h.StockCode.Contains(input.StockCode))
                .WhereIF(!string.IsNullOrEmpty(input.StockName), h => h.StockName.Contains(input.StockName))
                .OrderByIF(!string.IsNullOrEmpty(input.Sorting),input.Sorting)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),t=>t.CreationTime,OrderByType.Desc);
            
            var list = await query
                .Select(h => new StockHoldingDto
                {
                    Id = h.Id,
                    UserId = h.UserId,
                    StockId = h.StockId,
                    StockCode = h.StockCode,
                    StockName = h.StockName,
                    Quantity = h.Quantity,
                    CreationTime = h.CreationTime
                })
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
                
            return new PagedResultDto<StockHoldingDto>(total, list);
        }
        
        /// <summary>
        /// 获取当前用户的交易记录
        /// </summary>
        [Authorize]
        [HttpGet("stock/user-transactions")]
        public async Task<PagedResultDto<StockTransactionDto>> GetUserTransactionsAsync(StockTransactionGetListInputDto input)
        {
            Guid userId = CurrentUser.GetId();
            RefAsync<int> total = 0;
            
            var query = _stockTransactionRepository._DbQueryable
                .Where(t => t.UserId == userId)
                .WhereIF(!string.IsNullOrEmpty(input.StockCode), t => t.StockCode.Contains(input.StockCode))
                .WhereIF(!string.IsNullOrEmpty(input.StockName), t => t.StockName.Contains(input.StockName))
                .WhereIF(input.TransactionType.HasValue, t => t.TransactionType == input.TransactionType.Value)
                .WhereIF(input.StartTime.HasValue, t => t.CreationTime >= input.StartTime.Value)
                .WhereIF(input.EndTime.HasValue, t => t.CreationTime <= input.EndTime.Value)
                .OrderByIF(!string.IsNullOrEmpty(input.Sorting),input.Sorting)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),t=>t.CreationTime,OrderByType.Desc);
                
            var list = await query
                .Select(t => new StockTransactionDto
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    StockId = t.StockId,
                    StockCode = t.StockCode,
                    StockName = t.StockName,
                    TransactionType = t.TransactionType,
                    Price = t.Price,
                    Quantity = t.Quantity,
                    TotalAmount = t.TotalAmount,
                    Fee = t.Fee,
                    CreationTime = t.CreationTime
                })
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
                
            return new PagedResultDto<StockTransactionDto>(total, list);
        }
    }
} 