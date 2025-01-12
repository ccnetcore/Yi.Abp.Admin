using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yi.Abp.Tool;
class Program
{
    static async Task Main(string[] args)
    {

#if DEBUG
        
        //帮助
        //args = ["-h"];
        
        //版本
        // args = ["-v"];
        
        //清理
        // args = ["clear"];
        
        //创建模块
        //args = ["new","oooo", "-t","module","-p","D:\\temp","-csf"];
        
        //查看模板列表
        //args = ["new","list"];
        //查看子命令帮组
        // args = ["new","-h"];

        //添加模块
        //args = ["add-module", "kkk"];
#endif
        try
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureServices(async (host, service) =>
                {
                    await service.AddApplicationAsync<YiAbpToolModule>();
                })
                //.ConfigureAppConfiguration(configurationBuilder =>
                //{
                //    configurationBuilder.AddJsonFile("appsettings.json");
                //})
                .UseAutofac()
                .Build();
            var commandSelector = host.Services.GetRequiredService<CommandInvoker>();
            await commandSelector.InvokerAsync(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }

}