using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities
{
    [SugarTable("Setting")]
    public class SettingAggregateRoot : AggregateRoot<Guid>
    {

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public int CommentPage { get; set; }
        public int DiscussPage { get; set; }
        public int CommentExperience { get; set; }
        public int DiscussExperience { get; set; }
        public string Title { get; set; }
    }
}
