using Serilog;
using Serilog.Events;
using Yi.Abp.Web;

//创建日志,可使用{SourceContext}记录
Log.Logger = new LoggerConfiguration()
.MinimumLevel.Debug()
.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
.MinimumLevel.Override("Microsoft.AspNetCore.Hosting.Diagnostics", LogEventLevel.Error)
.MinimumLevel.Override("Quartz", LogEventLevel.Warning)
.Enrich.FromLogContext()
.WriteTo.Async(c => c.File("logs/all/log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Debug))
.WriteTo.Async(c => c.File("logs/error/errorlog-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Error))
.WriteTo.Async(c => c.Console())
.CreateLogger();

try
{
    Log.Information("""

        __     ___   ______                                           _    
        \ \   / (_) |  ____|                                         | |   
         \ \_/ / _  | |__ _ __ __ _ _ __ ___   _____      _____  _ __| | __
          \   / | | |  __| '__/ _` | '_ ` _ \ / _ \ \ /\ / / _ \| '__| |/ /
           | |  | | | |  | | | (_| | | | | | |  __/\ V  V / (_) | |  |   < 
           |_|  |_| |_|  |_|  \__,_|_| |_| |_|\___| \_/\_/ \___/|_|  |_|\_\
   
     """);
    Log.Information("Yi框架-Abp.vNext，启动！");

    var builder = WebApplication.CreateBuilder(args);
    Log.Information($"当前主机启动环境-【{builder.Environment.EnvironmentName}】");
    Log.Information($"当前主机启动地址-【{builder.Configuration["App:SelfUrl"]}】");
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