using Serilog;
using Serilog.Events;
using Yi.Abp.Web;

//创建日志,可使用{SourceContext}记录
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
.MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics",LogEventLevel.Error)
.MinimumLevel.Override("Quartz", LogEventLevel.Warning)
.Enrich.FromLogContext()
.WriteTo.Async(c => c.File("logs/log-.txt", rollingInterval: RollingInterval.Day))
.WriteTo.Async(c => c.Console())
.CreateLogger();

try
{
    Log.Information("Yi框架-Abp.vNext，启动！");

    var builder = WebApplication.CreateBuilder(args);
    builder.WebHost.UseUrls(builder.Configuration["App:SelfUrl"]);
    builder.Host.UseAutofac();
    builder.Host.UseSerilog();
    await builder.Services.AddApplicationAsync<YiAbpWebModule>();
    var app = builder.Build();
    await app.InitializeApplicationAsync();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Yi框架-Abp.vNext，爆炸！");
}
finally
{
    Log.CloseAndFlush();
}