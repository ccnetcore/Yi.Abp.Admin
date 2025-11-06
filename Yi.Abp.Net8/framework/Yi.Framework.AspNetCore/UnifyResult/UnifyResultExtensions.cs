using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.Response;
using Yi.Framework.AspNetCore.UnifyResult.Fiters;

namespace Yi.Framework.AspNetCore.UnifyResult;

/// <summary>
/// 规范化接口
/// 由于太多人反应，想兼容一套类似furion的返回情况，200状态码包一层更符合国内习惯，既然如此，不如直接搬过来
/// </summary>
public static class UnifyResultExtensions
{
    public static IServiceCollection AddFurionUnifyResultApi(this IServiceCollection services)
    {
        //成功规范接口
        services.AddTransient<SucceededUnifyResultFilter>();
        //异常规范接口
        services.AddTransient<FriendlyExceptionFilter>();
        services.AddMvc(options =>
        {
            options.Filters.RemoveAll(x => (x as ServiceFilterAttribute)?.ServiceType == typeof(AbpExceptionFilter));
            options.Filters.RemoveAll(x => (x as ServiceFilterAttribute)?.ServiceType == typeof(AbpNoContentActionFilter));
            options.Filters.AddService<SucceededUnifyResultFilter>(99);
            options.Filters.AddService<FriendlyExceptionFilter>(100);
        });
        return services;
    }
}