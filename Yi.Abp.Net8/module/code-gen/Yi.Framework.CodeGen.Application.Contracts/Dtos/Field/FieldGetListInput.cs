using Volo.Abp.Application.Dtos;

namespace Yi.Framework.CodeGen.Application.Contracts.Dtos.Field
{
    public class FieldGetListInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string? Name { get; set; }

        public Guid? TableId { get; set; }
    }
}
