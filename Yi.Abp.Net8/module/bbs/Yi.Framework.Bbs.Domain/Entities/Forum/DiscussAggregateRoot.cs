using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Entities.Forum
{
    [SugarTable("Discuss")]
    [SugarIndex($"index_{nameof(Title)}", nameof(Title), OrderByType.Asc)]
    [SugarIndex($"index_{nameof(CreationTime)}", nameof(CreationTime), OrderByType.Desc)]
    [SugarIndex($"index_{nameof(IsDeleted)}_{nameof(PlateId)}_{nameof(CreatorId)}",
        nameof(IsDeleted), OrderByType.Asc,
        nameof(PlateId), OrderByType.Asc,
        nameof(CreatorId), OrderByType.Asc
        )]
    public class DiscussAggregateRoot : AggregateRoot<Guid>, ISoftDelete, IAuditedObject
    {
        public DiscussAggregateRoot()
        {
        }
        public DiscussAggregateRoot(Guid plateId)
        {
            PlateId = plateId;
        }

        public void AddSeeNumber()
        {
            this.SeeNum += 1;
            //设置最小值，不更新
            this.LastModificationTime = DateTime.MinValue;
            //设置空值，不更新
            this.LastModifierId = Guid.Empty;
        }

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public string? Title { get; set; }
        public string? Introduction { get; set; }
        public int AgreeNum { get; set; }
        public int SeeNum { get; set; }
        
        /// <summary>
        /// 主题类型
        /// </summary>
        public DiscussTypeEnum DiscussType { get; set; }
        /// <summary>
        /// 封面
        /// </summary>
        public string? Cover { get; set; }

        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string Content { get; set; }

        public string? Color { get; set; }

        public bool IsDeleted { get; set; }

        //是否置顶，默认false
        public bool IsTop { get; set; }

        public int OrderNum { get; set; } = 0;


        public DiscussPermissionTypeEnum PermissionType { get; set; }

        public Guid PlateId { get; set; }
        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }


        /// <summary>
        /// 当PermissionType为角色时候，以下列表中的角色+创建者 代表拥有权限
        /// </summary>
        [SugarColumn(IsJson = true)] //使用json处理
        public List<string>? PermissionRoleCodes { get; set; } = new List<string>();

        [SugarColumn(IsJson = true)]//使用json处理
        public List<Guid>? DiscussLableIds{ get; set; }
        
        /// <summary>
        /// 是否禁止评论创建功能
        /// </summary>
        public bool IsDisableCreateComment { get; set; }
    }
}
