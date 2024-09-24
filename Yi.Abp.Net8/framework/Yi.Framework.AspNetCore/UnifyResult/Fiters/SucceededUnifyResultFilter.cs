using System.Collections;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Core.Extensions;

namespace Yi.Framework.AspNetCore.UnifyResult.Fiters;

/// <summary>
/// 规范化结构（请求成功）过滤器
/// </summary>
public class SucceededUnifyResultFilter : IAsyncActionFilter, IOrderedFilter
{
    /// <summary>
    /// 过滤器排序
    /// </summary>
    private const int FilterOrder = 8888;

    /// <summary>
    /// 排序属性
    /// </summary>
    public int Order => FilterOrder;

    /// <summary>
    /// 处理规范化结果
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 执行 Action 并获取结果
        var actionExecutedContext = await next();

        // 排除 WebSocket 请求处理
        if (actionExecutedContext.HttpContext.IsWebSocketRequest()) return;

        // 处理已经含有状态码结果的 Result
        if (actionExecutedContext.Result is IStatusCodeActionResult statusCodeResult &&
            statusCodeResult.StatusCode != null)
        {
            // 小于 200 或者 大于 299 都不是成功值，直接跳过
            if (statusCodeResult.StatusCode.Value < 200 || statusCodeResult.StatusCode.Value > 299)
            {
                // 处理规范化结果
                if (!CheckStatusCodeNonUnify(context.HttpContext, out var unifyRes))
                {
                    var httpContext = context.HttpContext;
                    var statusCode = statusCodeResult.StatusCode.Value;

                    // 解决刷新 Token 时间和 Token 时间相近问题
                    if (statusCodeResult.StatusCode.Value == StatusCodes.Status401Unauthorized
                        && httpContext.Response.Headers.ContainsKey("access-token")
                        && httpContext.Response.Headers.ContainsKey("x-access-token"))
                    {
                        httpContext.Response.StatusCode = statusCode = StatusCodes.Status403Forbidden;
                    }

                    // 如果 Response 已经完成输出，则禁止写入
                    if (httpContext.Response.HasStarted) return;
                    await unifyRes.OnResponseStatusCodes(httpContext, statusCode,
                        httpContext.RequestServices.GetService<IOptions<UnifyResultSettingsOptions>>()?.Value);
                }

                return;
            }
        }

        // 如果出现异常，则不会进入该过滤器
        if (actionExecutedContext.Exception != null) return;

        // 获取控制器信息
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 判断是否支持 MVC 规范化处理，检测配置而已
        // if (!UnifyContext.CheckSupportMvcController(context.HttpContext, actionDescriptor, out _)) return;

        // 判断是否跳过规范化处理，检测NonUnifyAttribute而已
        if (CheckSucceededNonUnify(actionDescriptor.MethodInfo))
        {
            return;
        }
        IUnifyResultProvider  unifyResult = context.GetRequiredService<IUnifyResultProvider>();
        
        // 处理 BadRequestObjectResult 类型规范化处理
        if (actionExecutedContext.Result is BadRequestObjectResult badRequestObjectResult)
        {
            // 解析验证消息
            var validationMetadata = GetValidationMetadata(badRequestObjectResult.Value);

            var result = unifyResult.OnValidateFailed(context, validationMetadata);
            if (result != null) actionExecutedContext.Result = result;
        }
        else
        {
            IActionResult result = default;

            // 检查是否是有效的结果（可进行规范化的结果）
            if (CheckVaildResult(actionExecutedContext.Result, out var data))
            {
                result = unifyResult.OnSucceeded(actionExecutedContext, data);
            }

            // 如果是不能规范化的结果类型，则跳过
            if (result == null) return;

            actionExecutedContext.Result = result;
        }
    }
    
        /// <summary>
    /// 获取验证错误信息
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    private static ValidationMetadata GetValidationMetadata(object errors)
    {
        ModelStateDictionary _modelState = null;
        object validationResults = null;
        (string message, string firstErrorMessage, string firstErrorProperty) = (default, default, default);

        // 判断是否是集合类型
        if (errors is IEnumerable && errors is not string)
        {
            // 如果是模型验证字典类型
            if (errors is ModelStateDictionary modelState)
            {
                _modelState = modelState;
                // 将验证错误信息转换成字典并序列化成 Json
                validationResults = modelState.Where(u => modelState[u.Key].ValidationState == ModelValidationState.Invalid)
                        .ToDictionary(u => u.Key, u => modelState[u.Key].Errors.Select(c => c.ErrorMessage).ToArray());
            }
            // 如果是 ValidationProblemDetails 特殊类型
            else if (errors is ValidationProblemDetails validation)
            {
                validationResults = validation.Errors
                    .ToDictionary(u => u.Key, u => u.Value.ToArray());
            }
            // 如果是字典类型
            else if (errors is Dictionary<string, string[]> dicResults)
            {
                validationResults = dicResults;
            }

            message = JsonSerializer.Serialize(validationResults, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
            firstErrorMessage = (validationResults as Dictionary<string, string[]>).First().Value[0];
            firstErrorProperty = (validationResults as Dictionary<string, string[]>).First().Key;
        }
        // 其他类型
        else
        {
            validationResults = firstErrorMessage = message = errors?.ToString();
        }

        return new ValidationMetadata
        {
            ValidationResult = validationResults,
            Message = message,
            ModelState = _modelState,
            FirstErrorProperty = firstErrorProperty,
            FirstErrorMessage = firstErrorMessage
        };
    }
        
    /// <summary>
    /// 检查是否是有效的结果（可进行规范化的结果）
    /// </summary>
    /// <param name="result"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private bool CheckVaildResult(IActionResult result, out object data)
    {
        data = default;

        // 排除以下结果，跳过规范化处理
        var isDataResult = result switch
        {
            ViewResult => false,
            PartialViewResult => false,
            FileResult => false,
            ChallengeResult => false,
            SignInResult => false,
            SignOutResult => false,
            RedirectToPageResult => false,
            RedirectToRouteResult => false,
            RedirectResult => false,
            RedirectToActionResult => false,
            LocalRedirectResult => false,
            ForbidResult => false,
            ViewComponentResult => false,
            PageResult => false,
            NotFoundResult => false,
            NotFoundObjectResult => false,
            _ => true,
        };

        // 目前支持返回值 ActionResult
        if (isDataResult) data = result switch
        {
            // 处理内容结果
            ContentResult content => content.Content,
            // 处理对象结果
            ObjectResult obj => obj.Value,
            // 处理 JSON 对象
            JsonResult json => json.Value,
            _ => null,
        };

        return isDataResult;
    }
    
    
    /// <summary>
    /// 检查短路状态码（>=400）是否进行规范化处理
    /// </summary>
    /// <param name="context"></param>
    /// <param name="unifyResult"></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    internal static bool CheckStatusCodeNonUnify(HttpContext context, out IUnifyResultProvider unifyResult)
    {
        // 获取终点路由特性
        var endpointFeature = context.Features.Get<IEndpointFeature>();
        if (endpointFeature == null) return (unifyResult = null) == null;

        // 判断是否跳过规范化处理
        var isSkip = context.GetEndpoint()?.Metadata?.GetMetadata<NonUnifyAttribute>()!= null
                     || endpointFeature?.Endpoint?.Metadata?.GetMetadata<NonUnifyAttribute>() != null
                     || context.Request.Headers["accept"].ToString().Contains("odata.metadata=", StringComparison.OrdinalIgnoreCase)
                     || context.Request.Headers["accept"].ToString().Contains("odata.streaming=", StringComparison.OrdinalIgnoreCase);

        if (isSkip == true) unifyResult = null;
        else
        {
            unifyResult = context.RequestServices.GetRequiredService<IUnifyResultProvider>();
        }

        return unifyResult == null || isSkip;
    }
    
    /// <summary>
    /// 检查请求成功是否进行规范化处理
    /// </summary>
    /// <param name="method"></param>
    /// <param name="isWebRequest"></param>
    /// <returns>返回 true 跳过处理，否则进行规范化处理</returns>
    private  bool CheckSucceededNonUnify(MethodInfo method, bool isWebRequest = true)
    {
        // 判断是否跳过规范化处理
        var isSkip =  method.CustomAttributes.Any(x => typeof(NonUnifyAttribute).IsAssignableFrom(x.AttributeType) || typeof(ProducesResponseTypeAttribute).IsAssignableFrom(x.AttributeType) || typeof(IApiResponseMetadataProvider).IsAssignableFrom(x.AttributeType))
                     || method.ReflectedType.IsDefined(typeof(NonUnifyAttribute), true)
                     || method.DeclaringType.Assembly.GetName().Name.StartsWith("Microsoft.AspNetCore.OData");

        if (!isWebRequest)
        {
            return isSkip;
        }
        return isSkip;
    }
}