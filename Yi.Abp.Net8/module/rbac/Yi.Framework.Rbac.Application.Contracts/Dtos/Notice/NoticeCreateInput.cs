using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Notice
{
    /// <summary>
    /// Notice输入创建对象
    /// </summary>
    public class NoticeCreateInput
    {
        public string Title { get; set; }
        public NoticeTypeEnum Type { get; set; }
        public string Content { get; set; }
        public int OrderNum { get; set; }
        public bool State { get; set; }

    }
}
