using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Notice
{
    /// <summary>
    /// 查询参数
    /// </summary>
    public class NoticeGetListInput : PagedAllResultRequestDto
    {
        public string? Title { get; set; }
        public NoticeTypeEnum? Type { get; set; }
    }
}
