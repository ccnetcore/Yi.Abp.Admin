using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Reflection;

namespace Yi.Framework.AspNetCore.Mvc
{
    /// <summary>
    /// 自定义服务约定实现,用于处理API路由和HTTP方法约束
    /// </summary>
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IAbpServiceConvention))]
    public class YiServiceConvention : AbpServiceConvention
    {
        /// <summary>
        /// 初始化服务约定的新实例
        /// </summary>
        /// <param name="options">ABP AspNetCore MVC 配置选项</param>
        /// <param name="conventionalRouteBuilder">约定路由构建器</param>
        public YiServiceConvention(
            IOptions<AbpAspNetCoreMvcOptions> options,
            IConventionalRouteBuilder conventionalRouteBuilder) 
            : base(options, conventionalRouteBuilder)
        {
        }

        /// <summary>
        /// 配置选择器,处理路由和HTTP方法约束
        /// </summary>
        protected override void ConfigureSelector(
            string rootPath,
            string controllerName,
            ActionModel action,
            ConventionalControllerSetting? configuration)
        {
            // 移除空选择器
            RemoveEmptySelectors(action.Selectors);

            // 检查远程服务特性
            var remoteServiceAttr = ReflectionHelper
                .GetSingleAttributeOrDefault<RemoteServiceAttribute>(action.ActionMethod);
            if (remoteServiceAttr != null && !remoteServiceAttr.IsEnabledFor(action.ActionMethod))
            {
                return;
            }

            // 根据选择器是否存在执行不同的配置
            if (!action.Selectors.Any())
            {
                AddAbpServiceSelector(rootPath, controllerName, action, configuration);
            }
            else
            {
                NormalizeSelectorRoutes(rootPath, controllerName, action, configuration);
            }
        }

        /// <summary>
        /// 规范化选择器路由
        /// </summary>
        protected override void NormalizeSelectorRoutes(
            string rootPath,
            string controllerName,
            ActionModel action,
            ConventionalControllerSetting? configuration)
        {
            foreach (var selector in action.Selectors)
            {
                // 获取HTTP方法约束
                var httpMethod = GetOrCreateHttpMethod(selector, action, configuration);

                // 处理路由模板
                ConfigureRouteTemplate(selector, rootPath, controllerName, action, httpMethod, configuration);

                // 确保HTTP方法约束存在
                EnsureHttpMethodConstraint(selector, httpMethod);
            }
        }

        /// <summary>
        /// 获取或创建HTTP方法
        /// </summary>
        private string GetOrCreateHttpMethod(
            SelectorModel selector,
            ActionModel action,
            ConventionalControllerSetting? configuration)
        {
            return selector.ActionConstraints
                .OfType<HttpMethodActionConstraint>()
                .FirstOrDefault()?
                .HttpMethods?
                .FirstOrDefault()
                ?? SelectHttpMethod(action, configuration);
        }

        /// <summary>
        /// 配置路由模板
        /// </summary>
        private void ConfigureRouteTemplate(
            SelectorModel selector,
            string rootPath,
            string controllerName,
            ActionModel action,
            string httpMethod,
            ConventionalControllerSetting? configuration)
        {
            if (selector.AttributeRouteModel == null)
            {
                selector.AttributeRouteModel = CreateAbpServiceAttributeRouteModel(
                    rootPath,
                    controllerName,
                    action,
                    httpMethod,
                    configuration);
            }
            else
            {
                NormalizeAttributeRouteTemplate(selector, rootPath);
            }
        }

        /// <summary>
        /// 规范化特性路由模板
        /// </summary>
        private void NormalizeAttributeRouteTemplate(SelectorModel selector, string rootPath)
        {
            var template = selector.AttributeRouteModel.Template;
            if (!template.StartsWith("/"))
            {
                selector.AttributeRouteModel.Template = $"{rootPath}/{template}";
            }
        }

        /// <summary>
        /// 确保HTTP方法约束存在
        /// </summary>
        private void EnsureHttpMethodConstraint(SelectorModel selector, string httpMethod)
        {
            if (!selector.ActionConstraints.OfType<HttpMethodActionConstraint>().Any())
            {
                selector.ActionConstraints.Add(
                    new HttpMethodActionConstraint(new[] { httpMethod }));
            }
        }
    }
}
