using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.WebClientInfo;

namespace Yi.Framework.AspNetCore;

public class RealIpHttpContextWebClientInfoProvider : HttpContextWebClientInfoProvider
{
    public RealIpHttpContextWebClientInfoProvider(ILogger<HttpContextWebClientInfoProvider> logger,
        IHttpContextAccessor httpContextAccessor) : base(logger, httpContextAccessor)
    {
    }

    protected override string? GetClientIpAddress()
    {
        try
        {
            var httpContext = HttpContextAccessor.HttpContext;

            var headers = httpContext?.Request?.Headers;

            if (headers != null && headers.ContainsKey("X-Forwarded-For"))
            {
                httpContext.Connection.RemoteIpAddress =
                    IPAddress.Parse(headers["X-Forwarded-For"].FirstOrDefault());
            }

            return httpContext?.Connection?.RemoteIpAddress?.ToString();
        }
        catch (Exception ex)
        {
            Logger.LogException(ex, LogLevel.Warning);
            return null;
        }
    }
}