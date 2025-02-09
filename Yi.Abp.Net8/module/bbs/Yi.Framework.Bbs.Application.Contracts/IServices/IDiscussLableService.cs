using Yi.Framework.Bbs.Application.Contracts.Dtos.MyType;
using Yi.Framework.Ddd.Application.Contracts;
namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Label服务抽象
    /// </summary>
    public interface IDiscussLableService : IYiCrudAppService<DiscussLableOutputDto, DiscussLableGetListOutputDto, Guid, DiscussLableGetListInputVo, DiscussLableCreateInputVo, DiscussLableUpdateInputVo>
    {

    }
}
