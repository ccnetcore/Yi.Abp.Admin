using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

/// <summary>
/// Hangfire 仪表盘的令牌认证过滤器
/// </summary>
public sealed class YiTokenAuthorizationFilter : IDashboardAsyncAuthorizationFilter, ITransientDependency
{
    private const string BearerPrefix = "Bearer ";
    private const string TokenCookieKey = "Token";
    private const string HtmlContentType = "text/html";
    
    private readonly IServiceProvider _serviceProvider;
    private string _requiredUsername = "cc";
    private TimeSpan _tokenExpiration = TimeSpan.FromMinutes(10);

    /// <summary>
    /// 初始化令牌认证过滤器
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public YiTokenAuthorizationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// 设置需要的用户名
    /// </summary>
    /// <param name="username">允许访问的用户名</param>
    /// <returns>当前实例，支持链式调用</returns>
    public YiTokenAuthorizationFilter SetRequiredUsername(string username)
    {
        _requiredUsername = username ?? throw new ArgumentNullException(nameof(username));
        return this;
    }

    /// <summary>
    /// 设置令牌过期时间
    /// </summary>
    /// <param name="expiration">过期时间间隔</param>
    /// <returns>当前实例，支持链式调用</returns>
    public YiTokenAuthorizationFilter SetTokenExpiration(TimeSpan expiration)
    {
        _tokenExpiration = expiration;
        return this;
    }

    /// <summary>
    /// 授权验证
    /// </summary>
    /// <param name="context">仪表盘上下文</param>
    /// <returns>是否通过授权</returns>
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var currentUser = _serviceProvider.GetRequiredService<ICurrentUser>();

        if (!currentUser.IsAuthenticated)
        {
            SetChallengeResponse(httpContext);
            return false;
        }

        // 如果验证通过，设置 cookie
        var authorization = httpContext.Request.Headers.Authorization.ToString();
        if (!string.IsNullOrWhiteSpace(authorization) && authorization.StartsWith(BearerPrefix))
        {
            var token = authorization[BearerPrefix.Length..];
            SetTokenCookie(httpContext, token);
        }

        return currentUser.UserName == _requiredUsername;
    }

    /// <summary>
    /// 设置认证挑战响应
    /// 当用户未认证时，返回一个包含令牌输入表单的HTML页面
    /// </summary>
    /// <param name="httpContext">HTTP 上下文</param>
    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.ContentType = HtmlContentType;
        
        var html = @"
            <html>
            <head>
                <title>Hangfire Dashboard Authorization</title>
                <style>
                    body { font-family: Arial, sans-serif; margin: 40px; }
                    .container { max-width: 400px; margin: 0 auto; }
                    .form-group { margin-bottom: 15px; }
                    input[type='text'] { width: 100%; padding: 8px; }
                    button { background: #337ab7; color: white; border: none; padding: 10px 15px; cursor: pointer; }
                    button:hover { background: #286090; }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Authorization Required</h2>
                    <div class='form-group'>
                        <input type='text' id='token' placeholder='Enter your Bearer token...' />
                    </div>
                    <button onclick='authorize()'>Authorize</button>
                </div>
                <script>
                    function authorize() {
                        var token = document.getElementById('token').value;
                        if (token) {
                            document.cookie = 'Token=' + token + '; path=/';
                            window.location.reload();
                        }
                    }
                </script>
            </body>
            </html>";

        httpContext.Response.WriteAsync(html);
    }

    /// <summary>
    /// 设置令牌 Cookie
    /// </summary>
    /// <param name="httpContext">HTTP 上下文</param>
    /// <param name="token">令牌值</param>
    private void SetTokenCookie(HttpContext httpContext, string token)
    {
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.Now.Add(_tokenExpiration),
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax
        };

        httpContext.Response.Cookies.Append(TokenCookieKey, token, cookieOptions);
    }

    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return Task.FromResult(Authorize(context));
    }
}