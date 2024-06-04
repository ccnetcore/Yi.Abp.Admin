using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Rbac.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{

    /// <summary>
    /// 评论表
    /// </summary>
    [SugarTable("Comment")]
    [SugarIndex($"index_{nameof(DiscussId)}", nameof(DiscussId), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(ParentId)}", nameof(ParentId), OrderByType.Asc)]
    public class CommentAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
    {
        /// <summary>
        /// 采用二维数组方式，不使用树形方式
        /// </summary>
        public CommentAggregateRoot()
        {
        }

        public CommentAggregateRoot(Guid discussId)
        {
            DiscussId = discussId;
        }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public bool IsDeleted { get; set; }

        [SugarColumn(Length = 500)]
        public string Content { get; set; }

        public Guid DiscussId { get; set; }

        /// <summary>
        /// 被回复的CommentId
        /// </summary>
        public Guid ParentId { get; set; }
        public DateTime CreationTime { get; set; }

        public Guid RootId { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<CommentAggregateRoot> Children { get; set; } = new();


        /// <summary>
        /// 用户,评论人用户信息
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(CreatorId))]
        public UserAggregateRoot CreateUser { get; set; }

        /// <summary>
        /// 被评论的用户信息
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public UserAggregateRoot CommentedUser { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
