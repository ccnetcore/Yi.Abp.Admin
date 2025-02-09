using SqlSugar;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("DiscussLable")]
    public class DiscussLableAggregateRoot : AggregateRoot<Guid>, ISoftDelete
    {
        public bool IsDeleted { get; set; }

        public string Name { get; set; }
        public string? Color { get; set; }
        public string? BackgroundColor { get; set; }
    }
}
