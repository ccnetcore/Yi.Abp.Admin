namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dept
{
    public class DeptUpdateInputVo
    {
        public bool State { get; set; }
        public string DeptName { get; set; } = string.Empty;
        public string DeptCode { get; set; } = string.Empty;
        public string? Leader { get; set; }
        public Guid? ParentId { get; set; }=Guid.Empty;
        public string? Remark { get; set; }
    }
}
