namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dept
{
    /// <summary>
    /// Dept输入创建对象
    /// </summary>
    public class DeptCreateInputVo
    {
        public bool State { get; set; }
        public string DeptName { get; set; }
        public string DeptCode { get; set; } 
        public string? Leader { get; set; }
        public Guid? ParentId { get; set; }=Guid.Empty;
        public string? Remark { get; set; }
    }
}
