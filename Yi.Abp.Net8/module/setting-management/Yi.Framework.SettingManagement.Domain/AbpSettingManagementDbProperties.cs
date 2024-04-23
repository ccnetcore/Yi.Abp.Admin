using Volo.Abp.Data;

namespace Yi.Framework.SettingManagement.Domain;

public static class AbpSettingManagementDbProperties
{
    public static string DbTablePrefix { get; set; } = AbpCommonDbProperties.DbTablePrefix;

    public static string DbSchema { get; set; } = AbpCommonDbProperties.DbSchema;

    public const string ConnectionStringName = "AbpSettingManagement";
}
