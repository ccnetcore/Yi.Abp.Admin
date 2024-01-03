using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleGetListOutputDto : EntityDto<Guid>
    {
        //批量查询，不给内容，性能考虑
        //public string Content { get; set; }
        public string Name { get; set; }
        public Guid DiscussId { get; set; }

        public List<ArticleGetListOutputDto>? Children { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
