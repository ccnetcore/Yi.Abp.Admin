using Volo.Abp;
using Volo.Abp.Application.Services;

namespace Yi.Framework.Ddd.Application.Contracts
{
    public interface IDeletesAppService<in TKey> : IDeleteAppService< TKey> , IApplicationService, IRemoteService
    {
        Task DeleteAsync(IEnumerable<TKey> ids);
    }
}
