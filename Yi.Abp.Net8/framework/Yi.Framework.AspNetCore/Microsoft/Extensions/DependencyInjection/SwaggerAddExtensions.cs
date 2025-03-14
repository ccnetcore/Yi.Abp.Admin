using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Options;

namespace Yi.Framework.AspNetCore.Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Swagger生成器扩展类
    /// </summary>
    public static class SwaggerAddExtensions
    {
        /// <summary>
        /// 添加Yi框架的Swagger生成器服务
        /// </summary>
        /// <typeparam name="TProgram">程序入口类型</typeparam>
        /// <param name="services">服务集合</param>
        /// <param name="setupAction">自定义配置动作</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddYiSwaggerGen<TProgram>(
            this IServiceCollection services,
            Action<SwaggerGenOptions>? setupAction = null)
        {
            // 获取MVC配置选项
            var mvcOptions = services.GetPreConfigureActions<AbpAspNetCoreMvcOptions>().Configure();
            
            // 获取并去重远程服务名称
            var remoteServiceSettings = mvcOptions.ConventionalControllers
                .ConventionalControllerSettings
                .DistinctBy(x => x.RemoteServiceName);

            services.AddAbpSwaggerGen(
                options =>
                {
                    // 应用外部配置
                    setupAction?.Invoke(options);

                    // 配置API文档分组
                    ConfigureApiGroups(options, remoteServiceSettings);

                    // 配置API文档过滤器
                    ConfigureApiFilter(options, remoteServiceSettings);

                    // 配置Schema ID生成规则
                    options.CustomSchemaIds(type => type.FullName);

                    // 包含XML注释文档
                    IncludeXmlComments<TProgram>(options);

                    // 配置JWT认证
                    ConfigureJwtAuthentication(options);

                    // 添加自定义过滤器
                    ConfigureCustomFilters(options);
                }
            );

            return services;
        }

        /// <summary>
        /// 配置API分组
        /// </summary>
        private static void ConfigureApiGroups(
            SwaggerGenOptions options, 
            IEnumerable<ConventionalControllerSetting> settings)
        {
            foreach (var setting in settings.OrderBy(x => x.RemoteServiceName))
            {
                if (!options.SwaggerGeneratorOptions.SwaggerDocs.ContainsKey(setting.RemoteServiceName))
                {
                    options.SwaggerDoc(setting.RemoteServiceName, new OpenApiInfo 
                    { 
                        Title = setting.RemoteServiceName, 
                        Version = "v1" 
                    });
                }
            }
        }

        /// <summary>
        /// 配置API文档过滤器
        /// </summary>
        private static void ConfigureApiFilter(
            SwaggerGenOptions options,
            IEnumerable<ConventionalControllerSetting> settings)
        {
            options.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (apiDesc.ActionDescriptor is ControllerActionDescriptor controllerDesc)
                {
                    var matchedSetting = settings
                        .FirstOrDefault(x => x.Assembly == controllerDesc.ControllerTypeInfo.Assembly);
                    return matchedSetting?.RemoteServiceName == docName;
                }
                return false;
            });
        }

        /// <summary>
        /// 包含XML注释文档
        /// </summary>
        private static void IncludeXmlComments<TProgram>(SwaggerGenOptions options)
        {
            var basePath = Path.GetDirectoryName(typeof(TProgram).Assembly.Location);
            if (basePath is not null)
            {
                foreach (var xmlFile in Directory.GetFiles(basePath, "*.xml"))
                {
                    options.IncludeXmlComments(xmlFile, true);
                }
            }
        }

        /// <summary>
        /// 配置JWT认证
        /// </summary>
        private static void ConfigureJwtAuthentication(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("JwtBearer", new OpenApiSecurityScheme
            {
                Description = "请在此输入JWT Token",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            var scheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "JwtBearer" 
                }
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [scheme] = Array.Empty<string>()
            });
        }

        /// <summary>
        /// 配置自定义过滤器
        /// </summary>
        private static void ConfigureCustomFilters(SwaggerGenOptions options)
        {
            options.OperationFilter<TenantHeaderOperationFilter>();
            options.SchemaFilter<EnumSchemaFilter>();
        }
    }

    /// <summary>
    /// Swagger文档枚举字段显示过滤器
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// 应用枚举架构过滤器
        /// </summary>
        /// <param name="schema">OpenAPI架构</param>
        /// <param name="context">架构过滤器上下文</param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum) return;

            schema.Enum.Clear();
            schema.Type = "string";
            schema.Format = null;

            var enumDescriptions = new StringBuilder();
            foreach (var enumName in Enum.GetNames(context.Type))
            {
                var enumValue = (Enum)Enum.Parse(context.Type, enumName);
                var description = GetEnumDescription(enumValue);
                var enumIntValue = Convert.ToInt64(enumValue);

                schema.Enum.Add(new OpenApiString(enumName));
                enumDescriptions.AppendLine(
                    $"【枚举：{enumName}{(description is null ? string.Empty : $"({description})")}={enumIntValue}】");
            }
            schema.Description = enumDescriptions.ToString();
        }

        /// <summary>
        /// 获取枚举描述特性值
        /// </summary>
        private static string? GetEnumDescription(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : null;
        }
    }

    /// <summary>
    /// 租户头部参数过滤器
    /// </summary>
    public class TenantHeaderOperationFilter : IOperationFilter
    {
        /// <summary>
        /// 租户标识键名
        /// </summary>
        private const string TenantHeaderKey = "__tenant";

        /// <summary>
        /// 应用租户头部参数过滤器
        /// </summary>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = TenantHeaderKey,
                In = ParameterLocation.Header,
                Required = false,
                AllowEmptyValue = true,
                Description = "租户ID或租户名称（留空表示默认租户）"
            });
        }
    }
}