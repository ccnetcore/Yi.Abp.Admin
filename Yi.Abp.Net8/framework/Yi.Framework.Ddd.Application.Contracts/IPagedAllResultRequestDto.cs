using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Ddd.Application.Contracts
{
    /// <summary>
    /// 分页查询请求接口，包含时间范围和排序功能
    /// </summary>
    public interface IPagedAllResultRequestDto : IPageTimeResultRequestDto, IPagedAndSortedResultRequest
    {
    }
}
