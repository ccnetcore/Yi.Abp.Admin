using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("MyType")]
    public class MyTypeEntity : Entity<Guid>, ISoftDelete
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public bool IsDeleted { get; set; }

        public string Name { get; set; }
        public string? Color { get; set; }
        public string? BackgroundColor { get; set; }

        public Guid UserId { get; set; }
    }
}
