using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Config
{
    public class ConfigGetOutputDto : EntityDto<Guid>
    {
        public string ConfigName { get; set; } = string.Empty;
        public string ConfigKey { get; set; } = string.Empty;
        public string ConfigValue { get; set; } = string.Empty;
        public string? ConfigType { get; set; }
        public int OrderNum { get; set; }
        public string? Remark { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
