using Volo.Abp.Application.Dtos;
using Yi.Framework.CodeGen.Application.Contracts.Dtos.Field;

namespace Yi.Framework.CodeGen.Application.Contracts.Dtos.Table
{
    public class TableDto : EntityDto<Guid>
    {
        public Guid Id { get; set; }
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
        public List<FieldDto>? Fields { get; set; }
    }
}
