using Microsoft.Extensions.Options;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

/// <summary>
/// 租户配置包装器
/// </summary>
public class TenantConfigurationWrapper : ITransientDependency
{
    private readonly IAbpLazyServiceProvider _serviceProvider;
    
    private ICurrentTenant CurrentTenantService => 
        _serviceProvider.LazyGetRequiredService<ICurrentTenant>();
    
    private ITenantStore TenantStoreService => 
        _serviceProvider.LazyGetRequiredService<ITenantStore>();
    
    private DbConnOptions DbConnectionOptions => 
        _serviceProvider.LazyGetRequiredService<IOptions<DbConnOptions>>().Value;

    /// <summary>
    /// 构造函数
    /// </summary>
    public TenantConfigurationWrapper(IAbpLazyServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 获取租户配置信息
    /// </summary>
    public async Task<TenantConfiguration?> GetAsync()
    {
        if (!DbConnectionOptions.EnabledSaasMultiTenancy)
        {
            return await TenantStoreService.FindAsync(ConnectionStrings.DefaultConnectionStringName);
        }

        return await GetTenantConfigurationByCurrentTenant();
    }

    private async Task<TenantConfiguration?> GetTenantConfigurationByCurrentTenant()
    {
        // 通过租户ID查找
        if (CurrentTenantService.Id.HasValue)
        {
            var config = await TenantStoreService.FindAsync(CurrentTenantService.Id.Value);
            if (config == null)
            {
                throw new ApplicationException($"未找到租户信息,租户Id:{CurrentTenantService.Id}");
            }
            return config;
        }

        // 通过租户名称查找
        if (!string.IsNullOrEmpty(CurrentTenantService.Name))
        {
            var config = await TenantStoreService.FindAsync(CurrentTenantService.Name);
            if (config == null)
            {
                throw new ApplicationException($"未找到租户信息,租户名称:{CurrentTenantService.Name}");
            }
            return config;
        }

        // 返回默认配置
        return await TenantStoreService.FindAsync(ConnectionStrings.DefaultConnectionStringName);
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

