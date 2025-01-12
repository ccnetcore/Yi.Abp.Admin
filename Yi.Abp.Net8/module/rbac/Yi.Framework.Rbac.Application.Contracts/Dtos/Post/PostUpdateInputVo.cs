namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Post
{
    public class PostUpdateInputVo
    {
        public bool? State { get; set; }
        public int OrderNum { get; set; }
        public string PostCode { get; set; }
        public string PostName { get; set; }
        public string? Remark { get; set; }
    }
}
