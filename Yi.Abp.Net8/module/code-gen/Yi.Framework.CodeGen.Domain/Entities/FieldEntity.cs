using SqlSugar;
using Volo.Abp.Domain.Entities;
using Yi.Framework.CodeGen.Domain.Shared.Enums;

namespace Yi.Framework.CodeGen.Domain.Entities
{
    [SugarTable("YiField")]
    public class FieldEntity : Entity<Guid>
    {
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Name { get; set; }
        public string? Description { get; set; }

        public int OrderNum { get; set; }
        public int Length { get; set; }

        public FieldTypeEnum FieldType { get; set; }

        public Guid TableId { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsAutoAdd { get; set; }

        /// <summary>
        /// 是否公共
        /// </summary>
        public bool IsPublic { get; set; }
    }
}
