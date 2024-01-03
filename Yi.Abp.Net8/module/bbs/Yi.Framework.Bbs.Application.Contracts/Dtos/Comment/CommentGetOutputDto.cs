using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Comment
{
    /// <summary>
    /// 单返回，返回单条评论即可
    /// </summary>
    public class CommentGetOutputDto : EntityDto<Guid>
    {

        public DateTime? CreateTime { get; set; }
        public string Content { get; set; }

        public Guid DiscussId { get; set; }


        /// <summary>
        /// 用户id联表为用户对象
        /// </summary>

        public BbsUserGetOutputDto User { get; set; }
        /// <summary>
        /// 根节点的评论id
        /// </summary>
        public Guid RootId { get; set; }

        /// <summary>
        /// 被回复的CommentId
        /// </summary>
        public Guid ParentId { get; set; }

    }
}
