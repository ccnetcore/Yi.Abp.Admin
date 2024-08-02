using Volo.Abp.Application.Services;
using Volo.Abp.SettingManagement.Localization;

namespace Yi.Framework.SettingManagement.Application;

public abstract class SettingManagementAppServiceBase : ApplicationService
{
    protected SettingManagementAppServiceBase()
    {
        LocalizationResource = typeof(AbpSettingManagementResource);
    }
}
