using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore
{
    public static class SqlSugarCoreExtensions
    {
        /// <summary>
        /// 新增db对象，可支持多个
        /// </summary>
        /// <param name="service"></param>
        /// <param name="serviceLifetime"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddYiDbContext<TDbContext>(this IServiceCollection service, ServiceLifetime serviceLifetime = ServiceLifetime.Transient) where TDbContext : class, ISqlSugarDbContextDependencies
        {
            service.AddTransient<ISqlSugarDbContextDependencies, TDbContext>();
            return service;
        }
        
        /// <summary>
        /// 新增db对象，可支持多个
        /// </summary>
        /// <param name="service"></param>
        /// <param name="options"></param>
        /// <typeparam name="TDbContext"></typeparam>
        /// <returns></returns>
        public static IServiceCollection AddYiDbContext<TDbContext>(this IServiceCollection service, Action<DbConnOptions> options) where TDbContext : class, ISqlSugarDbContextDependencies
        {
            service.Configure<DbConnOptions>(options.Invoke);
            service.AddYiDbContext<TDbContext>();
            return service;
        }
    }
}
