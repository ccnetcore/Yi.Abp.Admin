using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Post
{
    public class PostGetOutputDto : EntityDto<Guid>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public Guid? CreatorId { get; set; }
        public bool State { get; set; }
        public string PostCode { get; set; } = string.Empty;
        public string PostName { get; set; } = string.Empty;
        public string? Remark { get; set; }

        public int OrderNum { get; set; }
    }
}
