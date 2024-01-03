using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss
{
    /// <summary>
    /// Discuss输入创建对象
    /// </summary>
    public class DiscussCreateInputVo
    {
        public string Title { get; set; }
        public string? Types { get; set; }
        public string? Introduction { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public string Content { get; set; }
        public string? Color { get; set; }

        public Guid PlateId { get; set; }

        /// <summary>
        /// 默认公开
        /// </summary>
        public DiscussPermissionTypeEnum PermissionType { get; set; } = DiscussPermissionTypeEnum.Public;
        /// <summary>
        /// 封面
        /// </summary>
        public string? Cover { get; set; }

        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 是否禁止评论创建功能
        /// </summary>
        public bool IsDisableCreateComment { get; set; }
    }
}
