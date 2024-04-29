using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NSubstitute.Extensions;

namespace Yi.Framework.Rbac.Test
{
    public class YiTestWebBase : YiTestBase
    {
        public HttpContext HttpContext { get; private set; }
        public YiTestWebBase() : base()
        {
            HttpContext httpContext = DefaultHttpContextAccessor.CurrentHttpContext;
            ConfigureHttpContext(httpContext);
            HttpContext = httpContext;
            IApplicationBuilder app = new ApplicationBuilder(ServiceProvider);
            RequestDelegate httpDelegate = app.Build();
            httpDelegate.Invoke(httpContext);
        }

        public override void ConfigureServices(HostBuilderContext host, IServiceCollection service)
        {
            service.Replace(new ServiceDescriptor(typeof(IHttpContextAccessor), typeof(DefaultHttpContextAccessor), ServiceLifetime.Singleton));
            base.ConfigureServices(host, service);
        }

        protected virtual void ConfigureHttpContext(HttpContext httpContext)
        {
            httpContext.Request.Path = "/test";
        }
    }
}
internal class DefaultHttpContextAccessor : IHttpContextAccessor
{
    internal static HttpContext? CurrentHttpContext { get; set; } = new DefaultHttpContext();
    public HttpContext? HttpContext { get => CurrentHttpContext; set => throw new NotImplementedException(); }
}
