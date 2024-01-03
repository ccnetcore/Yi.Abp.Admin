using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Repositories
{
    public interface IArticleRepository: ISqlSugarRepository<ArticleEntity,Guid>
    {
        Task<List<ArticleEntity>> GetTreeAsync(Expression<Func<ArticleEntity, bool>> where);
    }
}
