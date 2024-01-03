using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Comment
{
    /// <summary>
    /// 评论多反
    /// </summary>
    public class CommentGetListOutputDto : EntityDto<Guid>
    {

        public DateTime? CreationTime { get; set; }




        public string Content { get; set; }


        /// <summary>
        /// 主题id
        /// </summary>
        public Guid DiscussId { get; set; }

        public Guid ParentId { get; set; }

        public Guid RootId { get; set; }

        /// <summary>
        /// 用户,评论人用户信息
        /// </summary>
        public BbsUserGetOutputDto CreateUser { get; set; }


        public Guid? CreatorId { get; set; }

        /// <summary>
        /// 被评论的用户信息
        /// </summary>
        public BbsUserGetOutputDto CommentedUser { get; set; }


        /// <summary>
        /// 这个不是一个树形，而是存在一个二维数组，该Children只有在顶级时候，只有一层
        /// </summary>
        public List<CommentGetListOutputDto> Children { get; set; } = new List<CommentGetListOutputDto>();
    }
}
