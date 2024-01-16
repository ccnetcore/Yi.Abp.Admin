using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Task
{
    public class TaskGetListInput : PagedAllResultRequestDto
    {
        public string? JobId { get; set; }
        public string? GroupName { get; set; }
    }
}
