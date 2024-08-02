using Yi.Abp.Tool.Web;


var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls(builder.Configuration["App:SelfUrl"]);
builder.Host.UseAutofac();
await builder.Services.AddApplicationAsync<YiAbpToolWebModule>();
var app = builder.Build();

await app.InitializeApplicationAsync();
await app.RunAsync();