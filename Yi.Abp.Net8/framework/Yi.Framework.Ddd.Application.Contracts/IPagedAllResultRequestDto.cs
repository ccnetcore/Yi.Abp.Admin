using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Ddd.Application.Contracts
{
    public interface IPagedAllResultRequestDto : IPageTimeResultRequestDto, IPagedAndSortedResultRequest
    {
    }
}
