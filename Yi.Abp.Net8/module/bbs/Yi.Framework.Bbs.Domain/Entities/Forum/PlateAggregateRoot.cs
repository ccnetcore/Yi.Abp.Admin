using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp;
using Volo.Abp.Auditing;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("Plate")]
    public class PlateAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
    {

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Introduction { get; set; }
        public bool IsDeleted { get; set; }



        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }

        public int OrderNum { get; set; }

        /// <summary>
        /// 是否禁用创建主题，禁用后，只有管理员或者权限者能够发送
        /// </summary>
        public bool IsDisableCreateDiscuss { get; set; }
    }
}
