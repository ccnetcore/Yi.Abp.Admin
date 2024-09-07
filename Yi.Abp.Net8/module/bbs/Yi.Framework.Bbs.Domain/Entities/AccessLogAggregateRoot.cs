using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Entities
{
    [SugarTable("AccessLog")]
    public class AccessLogAggregateRoot : AggregateRoot<Guid>, IHasCreationTime,IHasModificationTime
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public long Number { get; set; }
        
        public AccessLogTypeEnum AccessLogType { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }
}
