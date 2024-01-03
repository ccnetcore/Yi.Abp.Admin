using Yi.Framework.Ddd;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dept
{
    public class DeptGetListInputVo : PagedAllResultRequestDto
    {
        public Guid Id { get; set; }
        public bool? State { get; set; }
        public string? DeptName { get; set; }
        public string? DeptCode { get; set; }
        public string? Leader { get; set; }

    }
}
