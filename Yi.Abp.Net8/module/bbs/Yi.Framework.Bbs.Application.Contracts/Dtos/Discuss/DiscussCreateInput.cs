using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss
{
    /// <summary>
    /// Discuss输入创建对象
    /// </summary>
    public class DiscussCreateInput
    {
        public DiscussTypeEnum DiscussType { get; set; }
        
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

        /// <summary>
        /// 标签
        /// </summary>
        public List<Guid>? DiscussLableIds { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<string>? PermissionRoleCodes { get; set; } = new List<string>();
        
        /// <summary>
        /// 悬赏类型主题
        /// </summary>
        public DiscussRewardCreateInput? RewardData { get; set; }
    }


    public class DiscussRewardCreateInput
    {
        /// <summary>
        /// 悬赏最小价值
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// 悬赏最大价值
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// 作者联系方式
        /// </summary>
        public string Contact { get; set; }
    }
}
