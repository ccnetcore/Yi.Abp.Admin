using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.CodeGen.Domain.Entities
{
    [SugarTable("YiTemplate")]
    public class TemplateAggregateRoot : AggregateRoot<Guid>
    {

        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }

        /// <summary>
        /// 模板字符串
        /// </summary>
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string TemplateStr { get; set; } = string.Empty;

        /// <summary>
        /// 生成路径
        /// </summary>
        public string BuildPath { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }
    }
}