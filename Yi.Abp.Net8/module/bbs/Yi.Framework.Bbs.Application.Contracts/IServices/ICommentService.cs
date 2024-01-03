using Yi.Framework.Bbs.Application.Contracts.Dtos.Comment;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Comment服务抽象
    /// </summary>
    public interface ICommentService : IYiCrudAppService<CommentGetOutputDto, CommentGetListOutputDto, Guid, CommentGetListInputVo, CommentCreateInputVo, CommentUpdateInputVo>
    {

    }
}
