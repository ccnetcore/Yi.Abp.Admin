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
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;
using Yi.Framework.AspNetCore.Mvc;
using Yi.Framework.Core;

namespace Yi.Framework.AspNetCore
{
    [DependsOn(typeof(YiFrameworkCoreModule)
        )]
    public class YiFrameworkAspNetCoreModule : AbpModule
    {

    }
}