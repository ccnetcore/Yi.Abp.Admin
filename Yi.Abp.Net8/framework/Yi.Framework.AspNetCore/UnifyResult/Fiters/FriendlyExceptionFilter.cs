// MIT 许可证
//
// 版权 © 2020-present 百小僧, 百签科技（广东）有限公司 和所有贡献者
//
// 特此免费授予任何获得本软件副本和相关文档文件（下称“软件”）的人不受限制地处置该软件的权利，
// 包括不受限制地使用、复制、修改、合并、发布、分发、转授许可和/或出售该软件副本，
// 以及再授权被配发了本软件的人如上的权利，须在下列条件下：
//
// 上述版权声明和本许可声明应包含在该软件的所有副本或实质成分中。
//
// 本软件是“如此”提供的，没有任何形式的明示或暗示的保证，包括但不限于对适销性、特定用途的适用性和不侵权的保证。
// 在任何情况下，作者或版权持有人都不对任何索赔、损害或其他责任负责，无论这些追责来自合同、侵权或其它行为中，
// 还是产生于、源于或有关于本软件以及本软件的使用或其它处置。

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Validation;
using Yi.Framework.Core.Extensions;

namespace Yi.Framework.AspNetCore.UnifyResult.Fiters;

/// <summary>
///     友好异常拦截器
/// </summary>
public sealed class FriendlyExceptionFilter : IAsyncExceptionFilter
{
    /// <summary>
    ///     异常拦截
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task OnExceptionAsync(ExceptionContext context)
    {
        // 排除 WebSocket 请求处理
        if (context.HttpContext.IsWebSocketRequest()) return;

        // 如果异常在其他地方被标记了处理，那么这里不再处理
        if (context.ExceptionHandled) return;

        // 解析异常信息
        var exceptionMetadata = GetExceptionMetadata(context);
        var unifyResult = context.GetRequiredService<IUnifyResultProvider>();
        // 执行规范化异常处理
        context.Result = unifyResult.OnException(context, exceptionMetadata);

        // 创建日志记录器
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<FriendlyExceptionFilter>>();

        var errorMsg = "";
        if (exceptionMetadata.Errors != null) errorMsg = "\n" + JsonConvert.SerializeObject(exceptionMetadata.Errors);


        // 记录拦截日常
        logger.LogError(context.Exception, context.Exception.Message + errorMsg);
    }

    /// <summary>
    ///     获取异常元数据
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static ExceptionMetadata GetExceptionMetadata(ActionContext context)
    {
        object errorCode = default;
        object originErrorCode = default;
        object errors = default;
        object data = default;
        var statusCode = StatusCodes.Status500InternalServerError;
        var isValidationException = false; // 判断是否是验证异常
        var isFriendlyException = false;

        // 判断是否是 ExceptionContext 或者 ActionExecutedContext
        var exception = context is ExceptionContext exContext
            ? exContext.Exception
            : context is ActionExecutedContext edContext
                ? edContext.Exception
                : default;

        if (exception is AbpValidationException validationException)
        {
            errors = validationException.ValidationErrors;
            isValidationException = true;
        }

        // 判断是否是友好异常
        if (exception is UserFriendlyException friendlyException)
        {
            var statusCode2 = 500;
            int.TryParse(friendlyException.Code, out statusCode2);
            isFriendlyException = true;
            errorCode = friendlyException.Code;
            originErrorCode = friendlyException.Code;
            statusCode = statusCode2 == 0 ? 403 : statusCode2;
            errors = friendlyException.Message;
            data = friendlyException.Data;
        }

        return new ExceptionMetadata
        {
            StatusCode = statusCode,
            ErrorCode = errorCode,
            OriginErrorCode = originErrorCode,
            Errors = errors,
            Data = data
        };
    }
}