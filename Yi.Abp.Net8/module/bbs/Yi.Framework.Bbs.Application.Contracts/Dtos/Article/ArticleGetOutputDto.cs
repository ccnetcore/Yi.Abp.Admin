using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Article
{
    public class ArticleGetOutputDto : EntityDto<Guid>
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public Guid DiscussId { get; set; }
        public Guid ParentId { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
