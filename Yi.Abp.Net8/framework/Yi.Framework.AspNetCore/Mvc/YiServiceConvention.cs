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
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IAbpServiceConvention))]
    public class YiServiceConvention : AbpServiceConvention
    {
        public YiServiceConvention(IOptions<AbpAspNetCoreMvcOptions> options, IConventionalRouteBuilder conventionalRouteBuilder) : base(options, conventionalRouteBuilder)
        {
        }

        protected override void ConfigureSelector(string rootPath, string controllerName, ActionModel action, ConventionalControllerSetting? configuration)
        {
            RemoveEmptySelectors(action.Selectors);

            var remoteServiceAtt = ReflectionHelper.GetSingleAttributeOrDefault<RemoteServiceAttribute>(action.ActionMethod);
            if (remoteServiceAtt != null && !remoteServiceAtt.IsEnabledFor(action.ActionMethod))
            {
                return;
            }

            if (!action.Selectors.Any())
            {
                AddAbpServiceSelector(rootPath, controllerName, action, configuration);
            }
            else
            {
                NormalizeSelectorRoutes(rootPath, controllerName, action, configuration);
            }
        }


        protected override void AddAbpServiceSelector(string rootPath, string controllerName, ActionModel action, ConventionalControllerSetting? configuration)
        {
            base.AddAbpServiceSelector(rootPath, controllerName, action, configuration);
        }

        protected override void NormalizeSelectorRoutes(string rootPath, string controllerName, ActionModel action, ConventionalControllerSetting? configuration)
        {
            foreach (var selector in action.Selectors)
            {
                var httpMethod = selector.ActionConstraints
                    .OfType<HttpMethodActionConstraint>()
                    .FirstOrDefault()?
                    .HttpMethods?
                    .FirstOrDefault();

                if (httpMethod == null)
                {
                    httpMethod = SelectHttpMethod(action, configuration);
                }

                if (selector.AttributeRouteModel == null)
                {
                    selector.AttributeRouteModel = CreateAbpServiceAttributeRouteModel(rootPath, controllerName, action, httpMethod, configuration);
                }
                else
                {
             
                    var template = selector.AttributeRouteModel.Template;
                    if (!template.StartsWith("/"))
                    {
                        var route = $"{rootPath}/{template}";
                        selector.AttributeRouteModel.Template = route;

                    }
                    
                }



                if (!selector.ActionConstraints.OfType<HttpMethodActionConstraint>().Any())
                {
                    selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { httpMethod }));
                }


            }
        }



    }
}
