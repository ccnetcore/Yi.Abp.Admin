using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Model;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Bbs.Domain.Managers.ArticleImport
{
    public abstract class AbstractArticleImport
    {
        public void SetLogger(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }
        protected ILoggerFactory LoggerFactory { get; set; }
        public virtual List<ArticleAggregateRoot> Import(Guid discussId, Guid articleParentId, List<FileObject> fileObjs)
        {
            var articles = Convert(fileObjs);
            var orderNum = 0;
            articles.ForEach(article =>
            {
                article.DiscussId = discussId;
                article.ParentId = articleParentId;
                article.OrderNum = ++orderNum;
            });
            return articles;
        }
        public abstract List<ArticleAggregateRoot> Convert(List<FileObject> fileObjs);
    }
}
