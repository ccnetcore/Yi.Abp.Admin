using Volo.Abp.Application.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Model;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    public interface IOnlineService
    {
      Task< PagedResultDto<OnlineUserModel>> GetListAsync(OnlineUserModel online);
    }
}
