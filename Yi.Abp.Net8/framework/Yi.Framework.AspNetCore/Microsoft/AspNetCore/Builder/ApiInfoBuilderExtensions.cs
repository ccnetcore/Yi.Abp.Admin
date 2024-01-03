using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Yi.Framework.AspNetCore.Microsoft.AspNetCore.Middlewares;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder
{
    public static class ApiInfoBuilderExtensions
    {
        public static IApplicationBuilder UseYiApiHandlinge([NotNull] this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiInfoMiddleware>();
            return app;

        }
    }
}
