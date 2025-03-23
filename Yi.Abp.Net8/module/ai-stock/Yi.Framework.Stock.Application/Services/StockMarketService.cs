using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockPrice;
using Yi.Framework.Stock.Application.Contracts.IServices;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.Stock.Domain.Managers;
using Mapster;

namespace Yi.Framework.Stock.Application.Services
{
    /// <summary>
    /// 股市服务实现
    /// </summary>
    public class StockMarketService : ApplicationService, IStockMarketService
    {
        private readonly ISqlSugarRepository<StockMarketAggregateRoot> _stockMarketRepository;
        private readonly ISqlSugarRepository<StockPriceRecordEntity> _stockPriceRecordRepository;
        private readonly StockMarketManager _stockMarketManager;

        public StockMarketService(
            ISqlSugarRepository<StockMarketAggregateRoot> stockMarketRepository,
            ISqlSugarRepository<StockPriceRecordEntity> stockPriceRecordRepository,
            StockMarketManager stockMarketManager)
        {
            _stockMarketRepository = stockMarketRepository;
            _stockPriceRecordRepository = stockPriceRecordRepository;
            _stockMarketManager = stockMarketManager;
        }

        /// <summary>
        /// 创建股市
        /// </summary>
        [HttpPost("stock/markets")]
        [Authorize]
        public async Task<StockMarketDto> CreateStockMarketAsync(CreateStockMarketInputDto input)
        {
            // 使用映射将输入DTO转换为实体
            var stockMarket = input.Adapt<StockMarketAggregateRoot>();
        
            // 保存到数据库
            var result = await _stockMarketRepository.InsertReturnEntityAsync(stockMarket);
            
            // 使用映射将实体转换为返回DTO
            return result.Adapt<StockMarketDto>();
        }

        /// <summary>
        /// 获取股市列表
        /// </summary>
        [HttpGet("stock/markets")]
        public async Task<PagedResultDto<StockMarketDto>> GetStockMarketListAsync(StockMarketGetListInputDto input)
        {
            RefAsync<int> total = 0;

            var query = _stockMarketRepository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.MarketCode), m => m.MarketCode.Contains(input.MarketCode))
                .WhereIF(!string.IsNullOrEmpty(input.MarketName), m => m.MarketName.Contains(input.MarketName))
                .WhereIF(input.State.HasValue, m => m.State == input.State.Value)
                .OrderByIF(!string.IsNullOrEmpty(input.Sorting),input.Sorting)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),m=>m.OrderNum,OrderByType.Asc)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),m=>m.CreationTime,OrderByType.Desc);

            var list = await query
                .Select(m => new StockMarketDto
                {
                    Id = m.Id,
                    MarketCode = m.MarketCode,
                    MarketName = m.MarketName,
                    Description = m.Description,
                    State = m.State,
                    CreationTime = m.CreationTime
                })
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            return new PagedResultDto<StockMarketDto>(total, list);
        }

        /// <summary>
        /// 获取股市价格记录看板
        /// </summary>
        [HttpGet("stock/price-records")]
        public async Task<PagedResultDto<StockPriceRecordDto>> GetStockPriceRecordListAsync(StockPriceRecordGetListInputDto input)
        {
            RefAsync<int> total = 0;

            var query = _stockPriceRecordRepository._DbQueryable
                .WhereIF(input.StockId.HasValue, p => p.StockId == input.StockId.Value)
                .WhereIF(input.StartTime.HasValue, p => p.RecordTime >= input.StartTime.Value)
                .WhereIF(input.EndTime.HasValue, p => p.RecordTime <= input.EndTime.Value)
                .WhereIF(input.PeriodType.HasValue, p => p.PeriodType == input.PeriodType.Value)
                .Where(x=>x.RecordTime<=DateTime.Now)
                .OrderByIF(!string.IsNullOrEmpty(input.Sorting),input.Sorting)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),p=>p.RecordTime);

            var list = await query
                .Select(p => new StockPriceRecordDto
                {
                    Id = p.Id,
                    StockId = p.StockId,
                    CreationTime = p.CreationTime,
                    RecordTime = p.RecordTime,
                    CurrentPrice = p.CurrentPrice,
                    Volume = p.Volume,
                    Turnover = p.Turnover,
                    PeriodType = p.PeriodType
                })
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            return new PagedResultDto<StockPriceRecordDto>(total, list);
        }

        /// <summary>
        /// 买入股票
        /// </summary>
        [HttpPost("stock/buy")]
        [Authorize]
        public async Task BuyStockAsync(BuyStockInputDto input)
        {
            // 获取当前登录用户ID
            var userId = CurrentUser.GetId();
            
            // 调用领域服务进行股票购买
            await _stockMarketManager.BuyStockAsync(
                userId,
                input.StockId,
                input.Quantity
            );
        }

        /// <summary>
        /// 卖出股票
        /// </summary>
        [HttpDelete("stock/sell")]
        [Authorize]
        public async Task SellStockAsync(SellStockInputDto input)
        {
            // 获取当前登录用户ID
            var userId = CurrentUser.GetId();
            
            // 调用领域服务进行股票卖出
            await _stockMarketManager.SellStockAsync(
                userId,
                input.StockId,
                input.Quantity
            );
        }

        /// <summary>
        /// 生成最新股票记录
        /// </summary>
        [HttpPost("stock/generate")]
        [Authorize]
        public async Task GenerateStocksAsync()
        {
            await _stockMarketManager.GenerateStocksAsync();
        }

    }
} 