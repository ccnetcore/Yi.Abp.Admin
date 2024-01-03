using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Model;

namespace Yi.Framework.Bbs.Domain.Managers.ArticleImport
{
    public abstract class AbstractArticleImport
    {
        public virtual List<ArticleEntity> Import(Guid discussId,Guid articleParentId, List<FileObject> fileObjs)
        {
            var articles = Convert(fileObjs);
            articles.ForEach(article =>
            {
                article.DiscussId = discussId;
                article.ParentId = articleParentId;
            });
            return articles;
        }
        public abstract List<ArticleEntity> Convert(List<FileObject> fileObjs);
    }
}
