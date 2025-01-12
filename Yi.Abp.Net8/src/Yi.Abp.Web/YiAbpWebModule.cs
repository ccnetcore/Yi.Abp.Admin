using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Volo.Abp.AspNetCore.Auditing;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs.Hangfire;
using Volo.Abp.Caching;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Swashbuckle;
using Yi.Abp.Application;
using Yi.Abp.SqlsugarCore;
using Yi.Framework.AspNetCore;
using Yi.Framework.AspNetCore.Authentication.OAuth;
using Yi.Framework.AspNetCore.Authentication.OAuth.Gitee;
using Yi.Framework.AspNetCore.Authentication.OAuth.QQ;
using Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder;
using Yi.Framework.AspNetCore.Microsoft.Extensions.DependencyInjection;
using Yi.Framework.BackgroundWorkers.Hangfire;
using Yi.Framework.Bbs.Application;
using Yi.Framework.Bbs.Application.Extensions;
using Yi.Framework.ChatHub.Application;
using Yi.Framework.CodeGen.Application;
using Yi.Framework.Core.Json;
using Yi.Framework.Rbac.Application;
using Yi.Framework.Rbac.Domain.Authorization;
using Yi.Framework.Rbac.Domain.Shared.Consts;
using Yi.Framework.Rbac.Domain.Shared.Options;
using Yi.Framework.TenantManagement.Application;

namespace Yi.Abp.Web
{
    [DependsOn(
        typeof(YiAbpSqlSugarCoreModule),
        typeof(YiAbpApplicationModule),
        typeof(AbpAspNetCoreMultiTenancyModule),
        typeof(AbpAspNetCoreMvcModule),
        typeof(AbpAutofacModule),
        typeof(AbpSwashbuckleModule),
        typeof(AbpAspNetCoreSerilogModule),
        typeof(AbpAuditingModule),
        typeof(YiFrameworkBackgroundWorkersHangfireModule),
        typeof(AbpBackgroundJobsHangfireModule),
        typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
        typeof(YiFrameworkAspNetCoreModule),
        typeof(YiFrameworkAspNetCoreAuthenticationOAuthModule)
    )]
    public class YiAbpWebModule : AbpModule
    {
        private const string DefaultCorsPolicyName = "Default";

        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            //动态Api-改进在pre中配置，启动更快
            PreConfigure<AbpAspNetCoreMvcOptions>(options =>
            {
                options.ConventionalControllers.Create(typeof(YiAbpApplicationModule).Assembly,
                    options => options.RemoteServiceName = "default");
                options.ConventionalControllers.Create(typeof(YiFrameworkRbacApplicationModule).Assembly,
                    options => options.RemoteServiceName = "rbac");
                options.ConventionalControllers.Create(typeof(YiFrameworkBbsApplicationModule).Assembly,
                    options => options.RemoteServiceName = "bbs");
                options.ConventionalControllers.Create(typeof(YiFrameworkChatHubApplicationModule).Assembly,
                    options => options.RemoteServiceName = "chat-hub");
                options.ConventionalControllers.Create(
                    typeof(YiFrameworkTenantManagementApplicationModule).Assembly,
                    options => options.RemoteServiceName = "tenant-management");
                options.ConventionalControllers.Create(typeof(YiFrameworkCodeGenApplicationModule).Assembly,
                    options => options.RemoteServiceName = "code-gen");

                //统一前缀
                options.ConventionalControllers.ConventionalControllerSettings.ForEach(x => x.RootPath = "api/app");
            });
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            var host = context.Services.GetHostingEnvironment();
            var service = context.Services;

            //请求日志
            Configure<AbpAuditingOptions>(options =>
            {
                //默认关闭，开启会有大量的审计日志
                options.IsEnabled = true;
            });
            //忽略审计日志路径
            Configure<AbpAspNetCoreAuditingOptions>(options =>
            {
                options.IgnoredUrls.Add("/api/app/file/");
                options.IgnoredUrls.Add("/hangfire");
            });

            //采用furion格式的规范化api，默认不开启，使用abp优雅的方式
            //你没看错。。。
            //service.AddFurionUnifyResultApi();

            //配置错误处理显示详情
            Configure<AbpExceptionHandlingOptions>(options => { options.SendExceptionsDetailsToClients = true; });
            
            //【NewtonsoftJson严重问题！！！！！逆天】设置api格式，留给后人铭记
            // service.AddControllers().AddNewtonsoftJson(options =>
            // {
            //     options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            //     options.SerializerSettings.Converters.Add(new StringEnumConverter());
            // });

            //请使用微软的，注意abp date又包了一层，采用DefaultJsonTypeInfoResolver统一覆盖
            Configure<JsonOptions>(options =>
            {
                options.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver();
                options.JsonSerializerOptions.Converters.Add(new DatetimeJsonConverter());
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            //设置缓存不要过期，默认滑动20分钟
            Configure<AbpDistributedCacheOptions>(cacheOptions =>
            {
                cacheOptions.GlobalCacheEntryOptions.SlidingExpiration = null;
                //缓存key前缀
                cacheOptions.KeyPrefix = "Yi:";
            });


            Configure<AbpAntiForgeryOptions>(options => { options.AutoValidate = false; });

            //Swagger
            context.Services.AddYiSwaggerGen<YiAbpWebModule>(options =>
            {
                options.SwaggerDoc("default",
                    new OpenApiInfo { Title = "Yi.Framework.Abp", Version = "v1", Description = "集大成者" });
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

            //配置多租户
            Configure<AbpTenantResolveOptions>(options =>
            {
                //基于cookie jwt不好用，有坑
                options.TenantResolvers.Clear();
                options.TenantResolvers.Add(new HeaderTenantResolveContributor());
                //options.TenantResolvers.Add(new HeaderTenantResolveContributor());
                //options.TenantResolvers.Add(new CookieTenantResolveContributor());
                //options.TenantResolvers.RemoveAll(x => x.Name == CookieTenantResolveContributor.ContributorName);
            });

            //配置Hangfire定时任务存储，开启redis后，优先使用redis
            var redisConfiguration = configuration["Redis:Configuration"];
            var redisEnabled = configuration["Redis:IsEnabled"];
            context.Services.AddHangfire(config=>
            {
                if (redisEnabled.IsNullOrEmpty() || bool.Parse(redisEnabled))
                {
                    config.UseRedisStorage(
                        ConnectionMultiplexer.Connect(redisConfiguration),
                        new RedisStorageOptions()
                        {
                            InvisibilityTimeout = TimeSpan.FromHours(1), //JOB允许执行1小时
                            Prefix = "Yi:HangfireJob:"
                        }).WithJobExpirationTimeout(TimeSpan.FromHours(1));
                }
                else
                {
                    config.UseMemoryStorage();
                }
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


            //jwt鉴权
            var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
            var refreshJwtOptions = configuration.GetSection(nameof(RefreshJwtOptions)).Get<RefreshJwtOptions>();

            context.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            //优先Query中获取，再去cookies中获取
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            else
                            {
                                if (context.Request.Cookies.TryGetValue("Token", out var cookiesToken))
                                {
                                    context.Token = cookiesToken;
                                }
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddJwtBearer(TokenTypeConst.Refresh, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = refreshJwtOptions.Issuer,
                        ValidAudience = refreshJwtOptions.Audience,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(refreshJwtOptions.SecurityKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var refresh_token = context.Request.Headers["refresh_token"];
                            if (!string.IsNullOrEmpty(refresh_token))
                            {
                                context.Token = refresh_token;
                                return Task.CompletedTask;
                            }

                            var refreshToken = context.Request.Query["refresh_token"];
                            if (!string.IsNullOrEmpty(refreshToken))
                            {
                                context.Token = refreshToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddQQ(options => { configuration.GetSection("OAuth:QQ").Bind(options); })
                .AddGitee(options => { configuration.GetSection("OAuth:Gitee").Bind(options); });

            //授权
            context.Services.AddAuthorization();


            return Task.CompletedTask;
        }


        public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
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


            //无感token，先刷新再鉴权
            app.UseRefreshToken();

            //鉴权
            app.UseAuthentication();

            //多租户
            app.UseMultiTenancy();

            //swagger
            app.UseYiSwagger();

            //流量访问统计,需redis支持，否则不生效
            app.UseAccessLog();

            //请求处理
            app.UseYiApiHandlinge();

            //静态资源
            app.UseStaticFiles("/api/app/wwwroot");
            app.UseDefaultFiles();
            app.UseDirectoryBrowser("/api/app/wwwroot");


            // app.Properties.Add("_AbpExceptionHandlingMiddleware_Added",false);
            //工作单元
            app.UseUnitOfWork();

            //授权
            app.UseAuthorization();

            //审计日志
            app.UseAuditing();

            //日志记录
            app.UseAbpSerilogEnrichers();

            //Hangfire定时任务面板，可配置授权，意框架支持jwt
            app.UseAbpHangfireDashboard("/hangfire",
                options =>
                {
                    options.AsyncAuthorization = new[] { new YiTokenAuthorizationFilter(app.ApplicationServices) };
                });

            //终节点
            app.UseConfiguredEndpoints();
        }
    }
}