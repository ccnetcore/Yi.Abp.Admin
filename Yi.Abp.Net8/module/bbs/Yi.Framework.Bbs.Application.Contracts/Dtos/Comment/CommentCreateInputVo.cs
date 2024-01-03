namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Comment
{
    /// <summary>
    /// Comment输入创建对象
    /// </summary>
    public class CommentCreateInputVo
    {

        /// <summary>
        /// 评论id
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 主题id
        /// </summary>
        public Guid DiscussId { get; set; }

        /// <summary>
        /// 第一层评论id，第一层为0
        /// </summary>
        public Guid RootId { get; set; }

        /// <summary>
        /// 被回复的CommentId，第一层为0
        /// </summary>
        public Guid ParentId { get; set; }
    }
}
