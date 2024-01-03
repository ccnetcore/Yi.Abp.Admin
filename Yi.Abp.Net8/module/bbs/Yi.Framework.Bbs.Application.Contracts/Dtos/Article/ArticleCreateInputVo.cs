using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    /// <summary>
    /// Article输入创建对象
    /// </summary>
    public class ArticleCreateInputVo
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public Guid DiscussId { get; set; }
        public Guid ParentId { get; set; }
    }
}
