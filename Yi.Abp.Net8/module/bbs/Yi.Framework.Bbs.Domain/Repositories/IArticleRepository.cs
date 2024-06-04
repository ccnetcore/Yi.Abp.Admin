using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Repositories
{
    public interface IArticleRepository: ISqlSugarRepository<ArticleAggregateRoot,Guid>
    {
        Task<List<ArticleAggregateRoot>> GetTreeAsync(Expression<Func<ArticleAggregateRoot, bool>> where);
    }
}
