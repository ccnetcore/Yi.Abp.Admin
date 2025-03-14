using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Yi.Framework.AspNetCore.Mvc;
using Yi.Framework.Core;

namespace Yi.Framework.AspNetCore
{
    /// <summary>
    /// Yi框架ASP.NET Core模块
    /// </summary>
    [DependsOn(typeof(YiFrameworkCoreModule))]
    public class YiFrameworkAspNetCoreModule : AbpModule
    {
        /// <summary>
        /// 配置服务后的处理
        /// </summary>
        public override void PostConfigureServices(ServiceConfigurationContext context)
        {
            var services = context.Services;

            // 替换默认的WebClientInfoProvider为支持代理的实现
            services.Replace(new ServiceDescriptor(
                typeof(IWebClientInfoProvider),
                typeof(RealIpHttpContextWebClientInfoProvider), 
                ServiceLifetime.Transient));
        }
    }
}