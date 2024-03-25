using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yi.Abp.Client.Console;
using Yi.Framework.Rbac.Application.Contracts.IServices;

try
{
    IHost host = Host.CreateDefaultBuilder()
        .ConfigureServices(async (host, service) =>
        {
            await service.AddApplicationAsync<YiAbpClientConsoleModule>();
        })
        .UseAutofac()
        .Build();

    //控制台直接调用
    var account = host.Services.GetRequiredService<IAccountService>();

    //获取验证码
    var data1 = await account.GetCaptchaImageAsync();

    //登录
    var data2 = await account.PostLoginAsync(new Yi.Framework.Rbac.Application.Contracts.Dtos.Account.LoginInputVo { UserName = "cc", Password = "123456", Code = string.Empty, Uuid = string.Empty });


    host.Run();

}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}