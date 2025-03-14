using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Stock.Application.Contracts.Dtos.StockNews;

namespace Yi.Framework.Stock.Application.Contracts.IServices
{
    /// <summary>
    /// 股市新闻服务接口
    /// </summary>
    public interface IStockNewsService : IApplicationService
    {
        /// <summary>
        /// 获取股市新闻列表
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>新闻列表</returns>
        Task<PagedResultDto<StockNewsDto>> GetStockNewsListAsync(StockNewsGetListInputDto input);
    }
} 