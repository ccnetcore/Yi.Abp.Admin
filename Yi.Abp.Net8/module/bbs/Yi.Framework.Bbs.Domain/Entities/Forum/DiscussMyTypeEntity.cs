using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("DiscussMyType")]
    public class DiscussMyTypeEntity : Entity<Guid>
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        public Guid DiscussId { get; set; }

        public Guid MyTypeId { get; set; }
    }
}
