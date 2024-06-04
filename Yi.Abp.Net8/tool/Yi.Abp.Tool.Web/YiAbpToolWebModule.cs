using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Autofac;
using Volo.Abp.Swashbuckle;
using Yi.Abp.Tool.Application;
using Yi.Framework.AspNetCore;
using Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder;
using Yi.Framework.AspNetCore.Microsoft.Extensions.DependencyInjection;

namespace Yi.Abp.Tool.Web
{
    [DependsOn(typeof(YiAbpToolApplicationModule),


            typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule),
         typeof(YiFrameworkAspNetCoreModule)
        )]
    public class YiAbpToolWebModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";
        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var host = context.Services.GetHostingEnvironment();
            var service = context.Services;

            //动态Api
            Configure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(YiAbpToolApplicationModule).Assembly, options => options.RemoteServiceName = "tool");
                //统一前缀
                options.ConventionalControllers.ConventionalControllerSettings.ForEach(x => x.RootPath = "api/app");
            });

            //设置api格式
            service.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });




            Configure<AbpAntiForgeryOptions>(options =>
            {
                options.AutoValidate = false;
            });
            
            //Swagger
            context.Services.AddYiSwaggerGen<YiAbpToolWebModule>(options =>
            {
                options.SwaggerDoc("default", new OpenApiInfo { Title = "Yi.Framework.Abp", Version = "v1", Description = "集大成者" });
            });

            //跨域
            context.Services.AddCors(options =>
            {
                options.AddPolicy(DefaultCorsPolicyName, builder =>
                {
                    builder
                        .WithOrigins(
                            configuration["App:CorsOrigins"]!
                                .Split(";", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });


            //速率限制
            //每60秒限制100个请求，滑块添加，分6段
            service.AddRateLimiter(_ =>
            {
                _.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                _.OnRejected = (context, _) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");

                    return new ValueTask();
                };

                //全局使用，链式表达式
                _.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                   PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                   {
                       var userAgent = httpContext.Request.Headers.UserAgent.ToString();

                       return RateLimitPartition.GetSlidingWindowLimiter
                       (userAgent, _ =>
                           new SlidingWindowRateLimiterOptions
                           {
                               PermitLimit = 1000,
                               Window = TimeSpan.FromSeconds(60),
                               SegmentsPerWindow = 6,
                               QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                           });
                   }));
            });


         
            return Task.CompletedTask;
        }


        public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
        {
            var service = context.ServiceProvider;
            var env = context.GetEnvironment();
            var app = context.GetApplicationBuilder();

            app.UseRouting();

            //跨域
            app.UseCors(DefaultCorsPolicyName);

            if (!env.IsDevelopment())
            {
                //速率限制
                app.UseRateLimiter();
            }
            //swagger
            app.UseYiSwagger();

            //请求处理
            app.UseYiApiHandlinge();
            //静态资源
            app.UseStaticFiles("/api/app/wwwroot");
            app.UseDefaultFiles();
            app.UseDirectoryBrowser("/api/app/wwwroot");

            //终节点
            app.UseConfiguredEndpoints();

            return Task.CompletedTask;
        }
    }
}
