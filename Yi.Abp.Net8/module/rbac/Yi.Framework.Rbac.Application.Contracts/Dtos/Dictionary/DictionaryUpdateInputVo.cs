namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Dictionary
{
    public class DictionaryUpdateInputVo
    {
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
