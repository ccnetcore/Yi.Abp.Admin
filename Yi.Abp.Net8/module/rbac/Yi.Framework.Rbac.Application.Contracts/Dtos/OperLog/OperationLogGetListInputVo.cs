using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Domain.Shared.OperLog;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.OperLog
{
    public class OperationLogGetListInputVo : PagedAllResultRequestDto
    {
        public OperEnum? OperType { get; set; }
        public string? OperUser { get; set; }
    }
}
