using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dictionary
{
    public class DictionaryGetOutputDto : EntityDto<Guid>
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
        public Guid? CreatorId { get; set; }
        public string? Remark { get; set; }
        public string? ListClass { get; set; }
        public string? CssClass { get; set; }
        public string DictType { get; set; } = string.Empty;
        public string? DictLabel { get; set; }
        public string DictValue { get; set; } = string.Empty;
        public bool IsDefault { get; set; }

        public bool State { get; set; }
    }
}
