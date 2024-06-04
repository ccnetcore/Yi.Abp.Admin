using System.Linq.Expressions;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Repositories;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.Framework.Bbs.SqlSugarCore.Repositories
{
    public class ArticleRepository : SqlSugarRepository<ArticleAggregateRoot,Guid>, IArticleRepository,ITransientDependency
    {
        public ArticleRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
        {
        }

        public async Task<List<ArticleAggregateRoot>> GetTreeAsync(Expression<Func<ArticleAggregateRoot, bool>> where)
        {
            return await _DbQueryable.Where(where).OrderBy(x=>x.OrderNum).OrderBy(x=>x.CreationTime).ToTreeAsync(x => x.Children, x => x.ParentId, Guid.Empty);
        }
    }
}
