using Microsoft.AspNetCore.Builder;

namespace Yi.Framework.Bbs.Application.Extensions;

public static class AccessLogExtensions
{
    public static IApplicationBuilder UseAccessLog(this IApplicationBuilder app)
    {
        app.UseMiddleware<AccessLogMiddleware>();
        return app;
    }
}