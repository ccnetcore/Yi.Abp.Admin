using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("Banner")]
    public class BannerAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Color { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
