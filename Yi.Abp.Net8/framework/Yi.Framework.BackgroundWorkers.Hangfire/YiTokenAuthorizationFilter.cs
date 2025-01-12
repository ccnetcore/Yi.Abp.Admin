using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace Yi.Framework.BackgroundWorkers.Hangfire;

public class YiTokenAuthorizationFilter : IDashboardAsyncAuthorizationFilter, ITransientDependency
{
    private const string Bearer = "Bearer: ";
    private string RequireUser { get; set; } = "cc";
    private TimeSpan ExpiresTime { get; set; } = TimeSpan.FromMinutes(10);
    private IServiceProvider _serviceProvider;

    public YiTokenAuthorizationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public YiTokenAuthorizationFilter SetRequireUser(string userName)
    {
        RequireUser = userName;
        return this;
    }

    public YiTokenAuthorizationFilter SetExpiresTime(TimeSpan expiresTime)
    {
        ExpiresTime = expiresTime;
        return this;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var _currentUser = _serviceProvider.GetRequiredService<ICurrentUser>();
        //如果验证通过，设置cookies
        if (_currentUser.IsAuthenticated)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now + ExpiresTime, // 设置 cookie 过期时间,10分钟
            };


            var authorization = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                var token = httpContext.Request.Headers["Authorization"].ToString().Substring(Bearer.Length - 1);
                httpContext.Response.Cookies.Append("Token", token, cookieOptions);
            }

            if (_currentUser.UserName == RequireUser)
            {
                return true;
            }
        }

        SetChallengeResponse(httpContext);
        return false;
    }

    private void SetChallengeResponse(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = 401;
        httpContext.Response.ContentType = "text/html; charset=utf-8";
        string html = """
                      <!DOCTYPE html>
                      <html lang="zh">
                      <head>
                          <meta charset="UTF-8">
                          <meta name="viewport" content="width=device-width, initial-scale=1.0">
                          <title>Token 输入</title>
                          <script>
                              function sendToken() {
                                  // 获取输入的 token
                                  var token = document.getElementById("tokenInput").value;
                                  // 构建请求 URL
                                  var url = "/hangfire";
                                  // 发送 GET 请求
                                  fetch(url,{
                                      headers: {
                                         'Content-Type': 'application/json', // 设置内容类型为 JSON
                                         'Authorization': 'Bearer '+encodeURIComponent(token), // 设置授权头，例如使用 Bearer token
                                        },
                                      })
                                      .then(response => {
                                          if (response.ok) {
                                              return response.text(); // 或使用 response.json() 如果返回的是 JSON
                                          }
                                          throw new Error('Network response was not ok.');
                                      })
                                      .then(data => {
                                          // 处理成功返回的数据
                                           document.open();
                                           document.write(data);
                                           document.close();
                                      })
                                      .catch(error => {
                                          // 处理错误
                                          console.error('There has been a problem with your fetch operation:', error);
                                          alert("请求失败: " + error.message);
                                      });
                              }
                          </script>
                      </head>
                      <body style="text-align: center;">
                          <h1>Yi-hangfire</h1>
                          <h1>输入您的Token，我们将验证您是否为管理员</h1>
                          <input type="text" id="tokenInput" placeholder="请输入 token" />
                          <button onclick="sendToken()">校验</button>
                      </body>
                      </html>
                      """;
        httpContext.Response.WriteAsync(html);
    }

    public Task<bool> AuthorizeAsync(DashboardContext context)
    {
        return Task.FromResult(Authorize(context));
    }
}