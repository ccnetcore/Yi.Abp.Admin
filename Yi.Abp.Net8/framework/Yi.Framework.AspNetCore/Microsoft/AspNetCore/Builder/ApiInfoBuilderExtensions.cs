using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Yi.Framework.AspNetCore.Microsoft.AspNetCore.Middlewares;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 提供API信息处理的应用程序构建器扩展方法
    /// </summary>
    public static class ApiInfoBuilderExtensions
    {
        /// <summary>
        /// 使用Yi框架的API信息处理中间件
        /// </summary>
        /// <param name="builder">应用程序构建器实例</param>
        /// <returns>配置后的应用程序构建器实例</returns>
        /// <exception cref="ArgumentNullException">当builder参数为null时抛出</exception>
        public static IApplicationBuilder UseApiInfoHandling([NotNull] this IApplicationBuilder builder)
        {
            // 添加API信息处理中间件到请求管道
            builder.UseMiddleware<ApiInfoMiddleware>();
            
            return builder;
        }
    }
}
