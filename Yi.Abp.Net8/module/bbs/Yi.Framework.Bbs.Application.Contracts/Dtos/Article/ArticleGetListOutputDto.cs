using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleGetListOutputDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public Guid DiscussId { get; set; }

        public List<ArticleGetListOutputDto>? Children { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
