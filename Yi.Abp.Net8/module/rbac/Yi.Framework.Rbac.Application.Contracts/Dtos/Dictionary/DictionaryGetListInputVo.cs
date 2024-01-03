using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dictionary
{
    public class DictionaryGetListInputVo : PagedAndSortedResultRequestDto
    {
        public string? DictType { get; set; }
        public string? DictLabel { get; set; }
        public bool? State { get; set; }
    }
}
