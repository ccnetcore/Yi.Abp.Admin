using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

/// <summary>
/// 租户配置
/// </summary>
public class TenantConfigurationWrapper : ITransientDependency
{
    private readonly IAbpLazyServiceProvider _serviceProvider;
    private ICurrentTenant CurrentTenant => _serviceProvider.LazyGetRequiredService<ICurrentTenant>();
    private ITenantStore TenantStore => _serviceProvider.LazyGetRequiredService<ITenantStore>();
    private DbConnOptions DbConnOptions => _serviceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

    public TenantConfigurationWrapper(IAbpLazyServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 获取租户信息
    /// [from:ai]
    /// </summary>
    /// <returns></returns>
    public async Task<TenantConfiguration?> GetAsync()
    {
        //未开启多租户
        if (!DbConnOptions.EnabledSaasMultiTenancy)
        {
            return await TenantStore.FindAsync(ConnectionStrings.DefaultConnectionStringName);
        }
        
        TenantConfiguration? tenantConfiguration = null;
        
        if (CurrentTenant.Id is not null)
        {
            tenantConfiguration = await TenantStore.FindAsync(CurrentTenant.Id.Value);
            if (tenantConfiguration == null)
            {
                throw new ApplicationException($"未找到租户信息,租户Id:{CurrentTenant.Id}");
            }
            return tenantConfiguration;
        }
        
        if (!string.IsNullOrEmpty(CurrentTenant.Name))
        {
            tenantConfiguration = await TenantStore.FindAsync(CurrentTenant.Name);
            if (tenantConfiguration == null)
            {
                throw new ApplicationException($"未找到租户信息,租户名称:{CurrentTenant.Name}");
            }
            return tenantConfiguration;
        }
        
        return await TenantStore.FindAsync(ConnectionStrings.DefaultConnectionStringName);
    }

    /// <summary>
    /// 获取当前连接字符串
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetCurrentConnectionStringAsync()
    {
        return  (await GetAsync()).ConnectionStrings.Default!;
    }
    /// <summary>
    /// 获取当前连接名
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetCurrentConnectionNameAsync()
    {
        return  (await GetAsync()).Name;
    }
}

public static class TenantConfigurationExtensions
{
    /// <summary>
    /// 获取当前连接字符串
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentConnectionString(this TenantConfiguration tenantConfiguration)
    {
        return  tenantConfiguration.ConnectionStrings.Default!;
    }
    
    /// <summary>
    /// 获取当前连接名
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentConnectionName(this TenantConfiguration tenantConfiguration)
    {
        return  tenantConfiguration.Name;
    }
}

