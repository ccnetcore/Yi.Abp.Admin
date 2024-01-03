using Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Discuss服务抽象
    /// </summary>
    public interface IDiscussService : IYiCrudAppService<DiscussGetOutputDto, DiscussGetListOutputDto, Guid, DiscussGetListInputVo, DiscussCreateInputVo, DiscussUpdateInputVo>
    {
        Task VerifyDiscussPermissionAsync(Guid discussId);
    }
}
