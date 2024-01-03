using Yi.Framework.Bbs.Application.Contracts.Dtos.Article;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Article服务抽象
    /// </summary>
    public interface IArticleService : IYiCrudAppService<ArticleGetOutputDto, ArticleGetListOutputDto, Guid, ArticleGetListInputVo, ArticleCreateInputVo, ArticleUpdateInputVo>
    {

    }
}
