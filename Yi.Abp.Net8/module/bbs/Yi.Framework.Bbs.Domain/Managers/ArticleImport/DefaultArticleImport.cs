using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Model;

namespace Yi.Framework.Bbs.Domain.Managers.ArticleImport
{
    internal class DefaultArticleImport : AbstractArticleImport
    {
        public override List<ArticleEntity> Convert(List<FileObject> fileObjs)
        {
            return fileObjs.OrderBy(x => x.FileName).Select(x => new ArticleEntity { Name = x.FileName, Content = x.Content }).ToList();
        }
    }
}
