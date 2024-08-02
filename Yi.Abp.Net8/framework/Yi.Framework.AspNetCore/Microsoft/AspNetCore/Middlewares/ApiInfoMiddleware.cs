using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;
using Yi.Framework.Core.Extensions;
using static System.Net.WebRequestMethods;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Middlewares
{
    [DebuggerStepThrough]
    public class ApiInfoMiddleware : IMiddleware, ITransientDependency
    {
       
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            context.Response.OnStarting([DebuggerStepThrough] () =>
            {
                if (context.Response.StatusCode == StatusCodes.Status200OK
                && context.Response.Headers["Content-Type"].ToString() == "application/vnd.ms-excel")
                {
                    context.FileAttachmentHandle($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.xlsx");
                }
                if (context.Response.StatusCode == StatusCodes.Status200OK &&
                context.Response.Headers["Content-Type"].ToString() == "application/x-zip-compressed")
                {
                    context.FileAttachmentHandle($"{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.zip");
                }
                return Task.CompletedTask;
            });

            await next(context);



        }
    }
}
