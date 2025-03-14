using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace Yi.Framework.Core.Extensions
{
    /// <summary>
    /// HttpContext扩展方法类
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 设置内联文件下载响应头
        /// </summary>
        /// <param name="httpContext">HTTP上下文</param>
        /// <param name="fileName">文件名</param>
        public static void FileInlineHandle(this HttpContext httpContext, string fileName)
        {
            var encodeFilename = System.Web.HttpUtility.UrlEncode(fileName, Encoding.UTF8);
            httpContext.Response.Headers.Add("Content-Disposition", $"inline;filename={encodeFilename}");
        }

        /// <summary>
        /// 设置附件下载响应头
        /// </summary>
        /// <param name="httpContext">HTTP上下文</param>
        /// <param name="fileName">文件名</param>
        public static void FileAttachmentHandle(this HttpContext httpContext, string fileName)
        {
            var encodeFilename = System.Web.HttpUtility.UrlEncode(fileName, Encoding.UTF8);
            httpContext.Response.Headers.Add("Content-Disposition", $"attachment;filename={encodeFilename}");
        }

        /// <summary>
        /// 获取客户端首选语言
        /// </summary>
        /// <param name="httpContext">HTTP上下文</param>
        /// <returns>语言代码,默认返回zh-CN</returns>
        public static string GetLanguage(this HttpContext httpContext)
        {
            const string defaultLanguage = "zh-CN";
            var acceptLanguage = httpContext.Request.Headers["Accept-Language"].FirstOrDefault();
            
            return string.IsNullOrEmpty(acceptLanguage) 
                ? defaultLanguage 
                : acceptLanguage.Split(',')[0];
        }

        /// <summary>
        /// 判断是否为Ajax请求
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <returns>是否为Ajax请求</returns>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            const string ajaxHeader = "XMLHttpRequest";
            return ajaxHeader.Equals(request.Headers["X-Requested-With"], 
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>客户端IP地址</returns>
        public static string GetClientIp(this HttpContext context)
        {
            const string localhost = "127.0.0.1";
            if (context == null) return string.Empty;

            // 尝试获取X-Forwarded-For头
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            
            // 如果没有代理头,则获取远程IP
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress?.ToString();
            }

            // 处理特殊IP
            if (string.IsNullOrEmpty(ip) || ip.Contains("::1"))
            {
                return localhost;
            }

            // 清理IPv6格式
            ip = ip.Replace("::ffff:", localhost);
            
            // 移除端口号
            ip = Regex.Replace(ip, @":\d{1,5}$", "");

            // 验证IP格式
            var isValidIp = Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$") || 
                           Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?):\d{1,5}$");

            return isValidIp ? ip : localhost;
        }

        /// <summary>
        /// 获取User-Agent信息
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>User-Agent字符串</returns>
        public static string GetUserAgent(this HttpContext context)
        {
            return context.Request.Headers["User-Agent"].ToString();
        }

        /// <summary>
        /// 获取用户权限声明值
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <param name="permissionsName">权限声明名称</param>
        /// <returns>权限值数组</returns>
        public static string[]? GetUserPermissions(this HttpContext context, string permissionsName)
        {
            return context.User.Claims
                .Where(x => x.Type == permissionsName)
                .Select(x => x.Value)
                .ToArray();
        }
        
        /// <summary>
        /// 判断是否为WebSocket请求
        /// </summary>
        /// <param name="context">HTTP上下文</param>
        /// <returns>是否为WebSocket请求</returns>
        public static bool IsWebSocketRequest(this HttpContext context)
        {
            return context.WebSockets.IsWebSocketRequest || 
                   context.Request.Path == "/ws";
        }
    }
}
