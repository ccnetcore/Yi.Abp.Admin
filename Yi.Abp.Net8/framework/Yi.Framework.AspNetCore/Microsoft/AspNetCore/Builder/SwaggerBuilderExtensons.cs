using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Swagger构建器扩展类
    /// </summary>
    public static class SwaggerBuilderExtensions
    {
        /// <summary>
        /// 配置并使用Yi框架的Swagger中间件
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <param name="swaggerConfigs">Swagger配置模型数组</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseYiSwagger(
            this IApplicationBuilder app,
            params SwaggerConfiguration[] swaggerConfigs)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var mvcOptions = app.ApplicationServices
                .GetRequiredService<IOptions<AbpAspNetCoreMvcOptions>>()
                .Value;

            // 启用Swagger中间件
            app.UseSwagger();

            // 配置SwaggerUI
            app.UseSwaggerUI(options =>
            {
                // 添加约定控制器的Swagger终结点
                var conventionalSettings = mvcOptions.ConventionalControllers.ConventionalControllerSettings;
                foreach (var setting in conventionalSettings)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{setting.RemoteServiceName}/swagger.json",
                        setting.RemoteServiceName);
                }

                // 如果没有配置任何终结点，使用默认配置
                if (!conventionalSettings.Any() && (swaggerConfigs == null || !swaggerConfigs.Any()))
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Yi.Framework");
                    return;
                }

                // 添加自定义Swagger配置的终结点
                if (swaggerConfigs != null)
                {
                    foreach (var config in swaggerConfigs)
                    {
                        options.SwaggerEndpoint(config.Url, config.Name);
                    }
                }
            });

            return app;
        }
    }

    /// <summary>
    /// Swagger配置模型
    /// </summary>
    public class SwaggerConfiguration
    {
        private const string DefaultSwaggerUrl = "/swagger/v1/swagger.json";

        /// <summary>
        /// Swagger JSON文档的URL
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Swagger文档的显示名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 使用默认URL创建Swagger配置
        /// </summary>
        /// <param name="name">文档显示名称</param>
        public SwaggerConfiguration(string name)
            : this(DefaultSwaggerUrl, name)
        {
        }

        /// <summary>
        /// 创建自定义Swagger配置
        /// </summary>
        /// <param name="url">Swagger JSON文档URL</param>
        /// <param name="name">文档显示名称</param>
        public SwaggerConfiguration(string url, string name)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
