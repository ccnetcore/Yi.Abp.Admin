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
    [Dependency(ServiceLifetime.Transient, ReplaceServices = true)]
    [ExposeServices(typeof(IConventionalRouteBuilder))]
    public class YiConventionalRouteBuilder : ConventionalRouteBuilder
    {
        public YiConventionalRouteBuilder(IOptions<AbpConventionalControllerOptions> options) : base(options)
        {
        }
        public override string Build(
    string rootPath,
    string controllerName,
    ActionModel action,
    string httpMethod,
    [CanBeNull] ConventionalControllerSetting configuration)
        {

            var apiRoutePrefix = GetApiRoutePrefix(action, configuration);
            var controllerNameInUrl =
                NormalizeUrlControllerName(rootPath, controllerName, action, httpMethod, configuration);

            var url = $"{rootPath}/{NormalizeControllerNameCase(controllerNameInUrl, configuration)}";

            //Add {id} path if needed
            var idParameterModel = action.Parameters.FirstOrDefault(p => p.ParameterName == "id");
            if (idParameterModel != null)
            {
                if (TypeHelper.IsPrimitiveExtended(idParameterModel.ParameterType, includeEnums: true))
                {
                    url += "/{id}";
                }
                else
                {
                    var properties = idParameterModel
                        .ParameterType
                        .GetProperties(BindingFlags.Instance | BindingFlags.Public);

                    foreach (var property in properties)
                    {
                        url += "/{" + NormalizeIdPropertyNameCase(property, configuration) + "}";
                    }
                }
            }

            //Add action name if needed
            var actionNameInUrl = NormalizeUrlActionName(rootPath, controllerName, action, httpMethod, configuration);
            if (!actionNameInUrl.IsNullOrEmpty())
            {
                url += $"/{NormalizeActionNameCase(actionNameInUrl, configuration)}";

                //Add secondary Id
                var secondaryIds = action.Parameters
                    .Where(p => p.ParameterName.EndsWith("Id", StringComparison.Ordinal)).ToList();
                if (secondaryIds.Count == 1)
                {
                    url += $"/{{{NormalizeSecondaryIdNameCase(secondaryIds[0], configuration)}}}";
                }
            }

            return url;
        }

    }
}
