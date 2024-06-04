namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Post
{
    public class PostUpdateInputVo
    {
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public Guid? CreatorId { get; set; }
        public bool? State { get; set; }
        public string PostCode { get; set; } = string.Empty;
        public string PostName { get; set; } = string.Empty;
        public string? Remark { get; set; }
    }
}
