using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

/// <summary>
/// SqlSugar Core扩展方法
/// </summary>
public static class SqlSugarCoreExtensions
{
    /// <summary>
    /// 添加数据库上下文
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="serviceLifetime">服务生命周期</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddYiDbContext<TDbContext>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient) 
        where TDbContext : class, ISqlSugarDbContextDependencies
    {
        services.Add(new ServiceDescriptor(
            typeof(ISqlSugarDbContextDependencies),
            typeof(TDbContext),
            serviceLifetime));

        return services;
    }

    /// <summary>
    /// 添加数据库上下文并配置选项
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    /// <param name="services">服务集合</param>
    /// <param name="configureOptions">配置选项委托</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddYiDbContext<TDbContext>(
        this IServiceCollection services,
        Action<DbConnOptions> configureOptions)
        where TDbContext : class, ISqlSugarDbContextDependencies
    {
        services.Configure(configureOptions);
        services.AddYiDbContext<TDbContext>();
        return services;
    }
}
