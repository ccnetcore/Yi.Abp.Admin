using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Notice
{
    public class NoticeUpdateInput
    {
        public string? Title { get; set; }
        public NoticeTypeEnum? Type { get; set; }
        public string? Content { get; set; }
        public int? OrderNum { get; set; }
        public bool? State { get; set; }
    }
}
