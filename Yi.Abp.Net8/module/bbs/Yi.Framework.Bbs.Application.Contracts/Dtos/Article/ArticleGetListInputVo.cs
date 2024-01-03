using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleGetListInputVo : PagedAllResultRequestDto
    {
        public string? Content { get; set; }
        public string? Name { get; set; }
        public Guid? DiscussId { get; set; }
        public Guid? ParentId { get; set; }
    }
}
