namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Config
{
    public class ConfigUpdateInputVo
    {
        public Guid Id { get; set; }
        public string ConfigName { get; set; } = string.Empty;
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? ConfigType { get; set; }
        public int OrderNum { get; set; }
        public string? Remark { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
