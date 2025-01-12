using IPTools.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
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

        private IUnitOfWorkManager _unitOfWorkManager;
        //注入一个日志服务
        public OperLogGlobalAttribute(ILogger<OperLogGlobalAttribute> logger, IRepository<OperationLogEntity> repository, ICurrentUser currentUser, IUnitOfWorkManager unitOfWorkManager)
        {
            _logger = logger;
            _repository = repository;
            _currentUser = currentUser;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            //执行后

            //判断标签是在方法上
            if (resultContext.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor) return;

            //查找标签，获取标签对象
            OperLogAttribute? operLogAttribute = controllerActionDescriptor.MethodInfo
                .GetCustomAttributes(inherit: true)
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
            string location = "";
            try
            {
                var ipTool = IpTool.Search(ip);
                location = ipTool.Province + " " + ipTool.City;
            }
            catch
            {
                location = "搜索地址失败，可能是内网地址:" + ip;
            }


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
                logEntity.RequestParam = JsonConvert.SerializeObject(context.ActionArguments);
            }

            using (var uow = _unitOfWorkManager.Begin())
            {
                await _repository.InsertAsync(logEntity);
            }
        }

    }
}
