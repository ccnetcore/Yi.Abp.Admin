using IPTools.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Yi.Framework.Core.Extensions;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Domain.Shared.OperLog;

namespace Yi.Framework.Rbac.Domain.Operlog
{
    public class OperLogGlobalAttribute : ActionFilterAttribute, ITransientDependency
    {
        private ILogger<OperLogGlobalAttribute> _logger;
        private IRepository<OperationLogEntity> _repository;
        private ICurrentUser _currentUser;
        //注入一个日志服务
        public OperLogGlobalAttribute(ILogger<OperLogGlobalAttribute> logger, IRepository<OperationLogEntity> repository, ICurrentUser currentUser)
        {
            _logger = logger;
            _repository = repository;
            _currentUser = currentUser;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var resultContext = await next.Invoke();
            //执行后

            //判断标签是在方法上
            if (resultContext.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

            //查找标签，获取标签对象
            OperLogAttribute? operLogAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes(inherit: true)
                  .FirstOrDefault(a => a.GetType().Equals(typeof(OperLogAttribute))) as OperLogAttribute;
            //空对象直接返回
            if (operLogAttribute is null) return;

            ////获取控制器名
            //string controller = context.RouteData.Values["Controller"].ToString();

            ////获取方法名
            //string action = context.RouteData.Values["Action"].ToString();
            //获取Ip
            string ip = resultContext.HttpContext.GetClientIp();

            //根据ip获取地址

            var ipTool = IpTool.Search(ip);
            string location = ipTool.Province + " " + ipTool.City;

            //日志服务插入一条操作记录即可

            var logEntity = new OperationLogEntity();
            logEntity.OperIp = ip;
            //logEntity.OperLocation = location;
            logEntity.OperType = operLogAttribute.OperType;
            logEntity.Title = operLogAttribute.Title;
            logEntity.RequestMethod = resultContext.HttpContext.Request.Method;
            logEntity.Method = resultContext.HttpContext.Request.Path.Value;
            logEntity.OperLocation = location;
            logEntity.OperUser = _currentUser.UserName;
            if (operLogAttribute.IsSaveResponseData)
            {
                if (resultContext.Result is ContentResult result && result.ContentType == "application/json")
                {
                    logEntity.RequestResult = result.Content?.Replace("\r\n", "").Trim();
                }
                if (resultContext.Result is JsonResult result2)
                {
                    logEntity.RequestResult = result2.Value?.ToString();
                }

                if (resultContext.Result is ObjectResult result3)
                {
                    logEntity.RequestResult = JsonHelper.ObjToStr(result3.Value);
                }

            }

            if (operLogAttribute.IsSaveRequestData)
            {
                //不建议保存，吃性能
                //logEntity.RequestParam = context.HttpContext.GetRequestValue(logEntity.RequestMethod);
            }

            await _repository.InsertAsync(logEntity);


        }

    }
}
