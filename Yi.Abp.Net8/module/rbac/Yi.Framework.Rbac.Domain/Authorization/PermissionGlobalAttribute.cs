using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http;
using Yi.Framework.Core.Helper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yi.Framework.Rbac.Domain.Authorization
{
    internal class PermissionGlobalAttribute : ActionFilterAttribute, ITransientDependency
    {
        private readonly IPermissionHandler _permissionHandler;
        public PermissionGlobalAttribute(IPermissionHandler permissionHandler)
        {
            _permissionHandler = permissionHandler;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;
            List<PermissionAttribute>? perAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                   .Where(a => a.GetType().Equals(typeof(PermissionAttribute)))
                   .Select(x => x as PermissionAttribute)
                   .ToList()!;
            //空对象直接返回
            if (perAttributes.Count==0) return;

            var result = false;
            foreach (var perAttribute in perAttributes)
            {
                result = _permissionHandler.IsPass(perAttribute.Code);
                //存在有一个不满，直接跳出
                if (!result)
                {
                    break;
                }
            }


            if (!result)
            {
                var model = new RemoteServiceErrorInfo()
                {
                    Code = "403",
                    Message = $"您无权限访问,请联系管理员申请",
                    Details = $"您无权限访问该接口-{context.HttpContext.Request.Path.Value}",
                };

                var content = new ObjectResult(new { error = model })
                {
                    StatusCode = 403
                };
                context.Result = content;
                return;
            }
        }
    }
}