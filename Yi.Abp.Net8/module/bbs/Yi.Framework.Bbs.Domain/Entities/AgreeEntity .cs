using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities
{
    [SugarTable("Agree")]
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
