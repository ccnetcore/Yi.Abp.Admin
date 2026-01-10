using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Core.Extensions;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Middlewares
{
    /// <summary>
    /// API响应信息处理中间件
    /// 主要用于处理特定文件类型的响应头信息
    /// </summary>
    [DebuggerStepThrough]
    public class ApiInfoMiddleware : IMiddleware, ITransientDependency
    {
        /// <summary>
        /// 处理HTTP请求的中间件方法
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="next">请求处理委托</param>
        /// <returns>异步任务</returns>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // // 在响应开始时处理文件下载相关的响应头
            // context.Response.OnStarting(() =>
            // {
            //     HandleFileDownloadResponse(context);
            //     return Task.CompletedTask;
            // });

            // 继续处理管道中的下一个中间件
            await next(context);
        }

        /// <summary>
        /// 处理文件下载响应的响应头信息
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        private static void HandleFileDownloadResponse(HttpContext context)
        {
            // 仅处理状态码为200的响应
            if (context.Response.StatusCode != StatusCodes.Status200OK)
            {
                return;
            }

            var contentType = context.Response.Headers["Content-Type"].ToString();
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

            // 处理Excel文件下载
            if (contentType == "application/vnd.ms-excel")
            {
                context.FileAttachmentHandle($"{timestamp}.xlsx");
            }
            // 处理ZIP文件下载
            else if (contentType == "application/x-zip-compressed")
            {
                context.FileAttachmentHandle($"{timestamp}.zip");
            }
        }
    }
}
