using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Config
{
    public class ConfigGetListOutputDto : EntityDto<Guid>
    {
        public Guid Id { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        public string ConfigName { get; set; } = string.Empty;

        /// <summary>
        /// 配置主键
        /// </summary>
        public string ConfigKey { get; set; } = string.Empty;
        /// <summary>
        /// 配置值
        /// </summary>
        public string ConfigValue { get; set; } = string.Empty;
        /// <summary>
        /// 配置类型
        /// </summary>
        public string? ConfigType { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int OrderNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
