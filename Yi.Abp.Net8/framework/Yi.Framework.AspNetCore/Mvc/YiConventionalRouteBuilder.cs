using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace Yi.Framework.AspNetCore.Mvc
{
    /// <summary>
    /// 自定义路由构建器，用于生成API路由规则
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IConventionalRouteBuilder))]
    public class YiConventionalRouteBuilder : ConventionalRouteBuilder
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="options">ABP约定控制器配置选项</param>
        public YiConventionalRouteBuilder(IOptions<AbpConventionalControllerOptions> options) 
            : base(options)
        {
        }

        /// <summary>
        /// 构建API路由
        /// </summary>
        /// <param name="rootPath">根路径</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="action">Action模型</param>
        /// <param name="httpMethod">HTTP方法</param>
        /// <param name="configuration">控制器配置</param>
        /// <returns>构建的路由URL</returns>
        public override string Build(
            string rootPath,
            string controllerName,
            ActionModel action,
            string httpMethod,
            [CanBeNull] ConventionalControllerSetting configuration)
        {
            // 获取API路由前缀
            var apiRoutePrefix = GetApiRoutePrefix(action, configuration);
            
            // 规范化控制器名称
            var normalizedControllerName = NormalizeUrlControllerName(
                rootPath, 
                controllerName, 
                action, 
                httpMethod, 
                configuration);

            // 构建基础URL
            var url = $"{rootPath}/{NormalizeControllerNameCase(normalizedControllerName, configuration)}";

            // 处理ID参数路由
            url = BuildIdParameterRoute(url, action, configuration);

            // 处理Action名称路由
            url = BuildActionNameRoute(url, rootPath, controllerName, action, httpMethod, configuration);

            return url;
        }

        /// <summary>
        /// 构建ID参数路由部分
        /// </summary>
        private string BuildIdParameterRoute(
            string baseUrl, 
            ActionModel action, 
            ConventionalControllerSetting configuration)
        {
            var idParameter = action.Parameters.FirstOrDefault(p => p.ParameterName == "id");
            if (idParameter == null)
            {
                return baseUrl;
            }

            // 处理原始类型ID
            if (TypeHelper.IsPrimitiveExtended(idParameter.ParameterType, includeEnums: true))
            {
                return $"{baseUrl}/{{id}}";
            }

            // 处理复杂类型ID
            var properties = idParameter.ParameterType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties)
            {
                baseUrl += $"/{{{NormalizeIdPropertyNameCase(property, configuration)}}}";
            }

            return baseUrl;
        }

        /// <summary>
        /// 构建Action名称路由部分
        /// </summary>
        private string BuildActionNameRoute(
            string baseUrl,
            string rootPath,
            string controllerName,
            ActionModel action,
            string httpMethod,
            ConventionalControllerSetting configuration)
        {
            var actionNameInUrl = NormalizeUrlActionName(
                rootPath, 
                controllerName, 
                action, 
                httpMethod, 
                configuration);

            if (actionNameInUrl.IsNullOrEmpty())
            {
                return baseUrl;
            }

            baseUrl += $"/{NormalizeActionNameCase(actionNameInUrl, configuration)}";

            // 处理次要ID参数
            var secondaryIds = action.Parameters
                .Where(p => p.ParameterName.EndsWith("Id", StringComparison.Ordinal))
                .ToList();

            if (secondaryIds.Count == 1)
            {
                baseUrl += $"/{{{NormalizeSecondaryIdNameCase(secondaryIds[0], configuration)}}}";
            }

            return baseUrl;
        }
    }
}
