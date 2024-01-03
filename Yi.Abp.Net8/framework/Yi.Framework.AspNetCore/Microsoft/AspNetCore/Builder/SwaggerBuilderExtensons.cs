using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;

namespace Yi.Framework.AspNetCore.Microsoft.AspNetCore.Builder
{
    public static class SwaggerBuilderExtensons
    {
        public static IApplicationBuilder UseYiSwagger(this IApplicationBuilder app, params SwaggerModel[] swaggerModels)
        {
            var mvcOptions = app.ApplicationServices.GetRequiredService<IOptions<AbpAspNetCoreMvcOptions>>().Value;

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var setting in mvcOptions.ConventionalControllers.ConventionalControllerSettings)
                {
                    c.SwaggerEndpoint($"/swagger/{setting.RemoteServiceName}/swagger.json", setting.RemoteServiceName);
                }
                if (mvcOptions.ConventionalControllers.ConventionalControllerSettings.Count==0&&swaggerModels.Length == 0)
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Yi.Framework");
                }
                else
                {
                    foreach (var k in swaggerModels)
                    {
                        c.SwaggerEndpoint(k.Url, k.Name);
                    }
                }

            });
            return app;
        }

    }
    public class SwaggerModel
    {
        public SwaggerModel(string name)
        {
            this.Name = name;
            this.Url = "/swagger/v1/swagger.json";
        }
        public SwaggerModel(string url, string name)
        {
            this.Url = url;
            this.Name = name;
        }
        public string Url { get; set; }
        public string Name { get; set; }
    }
}
