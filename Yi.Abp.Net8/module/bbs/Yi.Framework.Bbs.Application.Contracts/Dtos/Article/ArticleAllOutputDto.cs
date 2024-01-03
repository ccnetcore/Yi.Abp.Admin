using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleAllOutputDto : EntityDto<Guid>
    {

        //批量查询，不给内容，性能考虑
        //public string Content { get; set; }
        public string Name { get; set; }
        public Guid DiscussId { get; set; }
        public Guid ParentId { get; set; }

        public List<ArticleAllOutputDto>? Children { get; set; }
    }
}
