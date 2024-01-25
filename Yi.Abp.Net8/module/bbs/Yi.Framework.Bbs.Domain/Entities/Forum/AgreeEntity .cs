using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("Agree")]
    [SugarIndex($"index_{nameof(CreatorId)}_{nameof(DiscussId)}", nameof(CreatorId), OrderByType.Asc,
        nameof(DiscussId), OrderByType.Asc)]
    public class AgreeEntity : Entity<Guid>, ICreationAuditedObject
    {
        public AgreeEntity()
        {
        }

        public AgreeEntity(Guid discussId)
        {
            DiscussId = discussId;
        }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 主题id
        /// </summary>
        public Guid DiscussId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public Guid? CreatorId { get; set; }

    }
}
