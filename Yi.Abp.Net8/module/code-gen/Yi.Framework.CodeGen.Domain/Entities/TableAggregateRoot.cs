using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.CodeGen.Domain.Entities
{
    [SugarTable("YiTable")]
    public class TableAggregateRoot : AggregateRoot<Guid>
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>

        public string? Description { get; set; }

        /// <summary>
        /// 一表多字段
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(FieldEntity.TableId))]
        public List<FieldEntity> Fields { get; set; }

        [SugarColumn(IsIgnore =true)]
        public override ExtraPropertyDictionary ExtraProperties { get; protected set; }
    }
}
