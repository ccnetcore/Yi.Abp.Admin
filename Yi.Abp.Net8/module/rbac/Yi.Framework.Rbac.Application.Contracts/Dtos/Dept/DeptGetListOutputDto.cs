using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dept
{
    public class DeptGetListOutputDto : EntityDto<Guid>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public Guid? CreatorId { get; set; }
        public bool State { get; set; }
        public string DeptName { get; set; } = string.Empty;
        public string DeptCode { get; set; } = string.Empty;
        public string? Leader { get; set; }
        public Guid ParentId { get; set; }
        public string? Remark { get; set; }

        public int OrderNum { get; set; }
    }
}
