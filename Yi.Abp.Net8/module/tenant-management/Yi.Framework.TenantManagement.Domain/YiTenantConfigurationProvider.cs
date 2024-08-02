using Microsoft.Extensions.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy.Localization;

namespace Volo.Abp.MultiTenancy;

[Dependency(ReplaceServices =true)]
public class YiTenantConfigurationProvider : ITenantConfigurationProvider, ITransientDependency
{
    protected virtual ITenantResolver TenantResolver { get; }
    protected virtual ITenantStore TenantStore { get; }
    protected virtual ITenantResolveResultAccessor TenantResolveResultAccessor { get; }
    protected virtual IStringLocalizer<AbpMultiTenancyResource> StringLocalizer { get; }

    public YiTenantConfigurationProvider(
        ITenantResolver tenantResolver,
        ITenantStore tenantStore,
        ITenantResolveResultAccessor tenantResolveResultAccessor,
        IStringLocalizer<AbpMultiTenancyResource> stringLocalizer)
    {
        TenantResolver = tenantResolver;
        TenantStore = tenantStore;
        TenantResolveResultAccessor = tenantResolveResultAccessor;
        StringLocalizer = stringLocalizer;
    }

    public virtual async Task<TenantConfiguration?> GetAsync(bool saveResolveResult = false)
    {
        //租户解析器获取到当前解析成功的租户
        var resolveResult = await TenantResolver.ResolveTenantIdOrNameAsync();

        if (saveResolveResult)
        {
            TenantResolveResultAccessor.Result = resolveResult;
        }

        TenantConfiguration? tenant = null;
        if (resolveResult.TenantIdOrName != null)
        {
            //根据租户信息获取租户
            tenant = await FindTenantAsync(resolveResult.TenantIdOrName);

            if (tenant == null)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.MultiTenancy:010001",
                    message: StringLocalizer["TenantNotFoundMessage"],
                    details: StringLocalizer["TenantNotFoundDetails", resolveResult.TenantIdOrName]
                );
            }

            if (!tenant.IsActive)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.MultiTenancy:010002",
                    message: StringLocalizer["TenantNotActiveMessage"],
                    details: StringLocalizer["TenantNotActiveDetails", resolveResult.TenantIdOrName]
                );
            }
        }

        return tenant;
    }

    protected virtual async Task<TenantConfiguration?> FindTenantAsync(string tenantIdOrName)
    {
        if (Guid.TryParse(tenantIdOrName, out var parsedTenantId))
        {
            return await TenantStore.FindAsync(parsedTenantId);
        }
        else
        {
            return await TenantStore.FindAsync(tenantIdOrName);
        }
    }
}
