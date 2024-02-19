using Volo.Abp.Application.Dtos;

namespace Yi.Framework.CodeGen.Application.Contracts.Dtos.Template
{
    public class TemplateGetListInput : PagedAndSortedResultRequestDto
    {

        /// <summary>
        /// 模板名称
        /// </summary>
        public string? Name { get; set; }

    }
}
