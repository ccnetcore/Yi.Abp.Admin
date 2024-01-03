using Volo.Abp.Application.Dtos;
using Yi.Framework.Rbac.Domain.Shared.OperLog;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.OperLog
{
    public class OperationLogGetListOutputDto : EntityDto<Guid>
    {
        public string? Title { get; set; }
        public OperEnum OperType { get; set; }
        public string? RequestMethod { get; set; }
        public string? OperUser { get; set; }
        public string? OperIp { get; set; }
        public string? OperLocation { get; set; }
        public string? Method { get; set; }
        public string? RequestParam { get; set; }
        public string? RequestResult { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
