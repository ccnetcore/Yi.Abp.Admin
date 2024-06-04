using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities
{
    [SugarTable("AccessLog")]
    public class AccessLogAggregateRoot : AggregateRoot<Guid>, IHasModificationTime, IHasCreationTime
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public long Number { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
