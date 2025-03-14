using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockNews;
using Yi.Framework.Stock.Application.Contracts.IServices;
using Yi.Framework.Stock.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.Stock.Domain.Managers;

namespace Yi.Framework.Stock.Application.Services
{
    /// <summary>
    /// 股市新闻服务实现
    /// </summary>
    public class StockNewsService : ApplicationService, IStockNewsService
    {
        private readonly ISqlSugarRepository<StockNewsAggregateRoot> _stockNewsRepository;
        private readonly NewsManager _newsManager;
        
        public StockNewsService(
            ISqlSugarRepository<StockNewsAggregateRoot> stockNewsRepository,
            NewsManager newsManager)
        {
            _stockNewsRepository = stockNewsRepository;
            _newsManager = newsManager;
        }
        
        /// <summary>
        /// 获取股市新闻列表
        /// </summary>
        [HttpGet("/api/app/stock/news")]
        public async Task<PagedResultDto<StockNewsDto>> GetStockNewsListAsync(StockNewsGetListInputDto input)
        {
            RefAsync<int> total = 0;
            
            // 计算10天前的日期
            DateTime tenDaysAgo = DateTime.Now.AddDays(-10);
            
            var query = _stockNewsRepository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.Title), n => n.Title.Contains(input.Title))
                .WhereIF(!string.IsNullOrEmpty(input.Source), n => n.Source.Contains(input.Source))
                .WhereIF(input.StartTime.HasValue, n => n.PublishTime >= input.StartTime.Value)
                .WhereIF(input.EndTime.HasValue, n => n.PublishTime <= input.EndTime.Value)
                // 如果IsRecent为true，则只查询最近10天的新闻
                .WhereIF(input.IsRecent, n => n.PublishTime >= tenDaysAgo)
                .OrderByIF(!string.IsNullOrEmpty(input.Sorting),input.Sorting)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),n=>n.OrderNum,OrderByType.Asc)
                .OrderByIF(string.IsNullOrEmpty(input.Sorting),n=>n.PublishTime,OrderByType.Desc) ;
            
            
            var list = await query
                .Select(n => new StockNewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishTime = n.PublishTime,
                    Source = n.Source,
                    CreationTime = n.CreationTime,
                    OrderNum = n.OrderNum
                })
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
                
            return new PagedResultDto<StockNewsDto>(total, list);
        }
        
        /// <summary>
        /// 生成股市新闻
        /// </summary>
        /// <returns>生成结果</returns>
        [HttpPost("/api/app/stock/news/generate")]
        public async Task GenerateNewsAsync()
        {
            await _newsManager.GenerateNewsAsync();
        }
    }
} 