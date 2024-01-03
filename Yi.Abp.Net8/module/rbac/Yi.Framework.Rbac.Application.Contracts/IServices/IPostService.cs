using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Post;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// Post服务抽象
    /// </summary>
    public interface IPostService : IYiCrudAppService<PostGetOutputDto, PostGetListOutputDto, Guid, PostGetListInputVo, PostCreateInputVo, PostUpdateInputVo>
    {

    }
}
