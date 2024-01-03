using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Core.Data;

namespace Yi.Framework.Rbac.Domain.Entities
{
    [SugarTable("Dictionary")]
    public class DictionaryEntity : Entity<Guid>, IAuditedObject, ISoftDelete, IOrderNum, IState
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }


        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int OrderNum { get; set; } = 0;

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; } = true;

        /// <summary>
        /// 描述 
        ///</summary>
        [SugarColumn(ColumnName = "Remark")]
        public string? Remark { get; set; }
        /// <summary>
        /// tag类型 
        ///</summary>
        [SugarColumn(ColumnName = "ListClass")]
        public string? ListClass { get; set; }
        /// <summary>
        /// tagClass 
        ///</summary>
        [SugarColumn(ColumnName = "CssClass")]
        public string? CssClass { get; set; }

        /// <summary>
        /// 字典类型 
        ///</summary>
        [SugarColumn(ColumnName = "DictType")]
        public string DictType { get; set; } = string.Empty;
        /// <summary>
        /// 字典标签 
        ///</summary>
        [SugarColumn(ColumnName = "DictLabel")]
        public string? DictLabel { get; set; }
        /// <summary>
        /// 字典值 
        ///</summary>
        [SugarColumn(ColumnName = "DictValue")]
        public string DictValue { get; set; } = string.Empty;
        /// <summary>
        /// 是否为该类型的默认值 
        ///</summary>
        [SugarColumn(ColumnName = "IsDefault")]
        public bool IsDefault { get; set; }

        public DateTime CreationTime { get; set; }

        public Guid? CreatorId { get; set; }

        public Guid? LastModifierId { get; set; }

        public DateTime? LastModificationTime { get; set; }
    }
}
