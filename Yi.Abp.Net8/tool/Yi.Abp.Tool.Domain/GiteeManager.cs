using System.Net;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Volo.Abp.DependencyInjection;

namespace Yi.Abp.Tool.Domain;

public class GiteeManager : ITransientDependency
{
    private readonly string _accessToken;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string GiteeHost = "https://gitee.com/api/v5";
    private const string Owner = "ccnetcore";
    private const string Repo = "yi-template";

    public GiteeManager(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _accessToken = configuration.GetValue<string>("GiteeAccession");
    }

    /// <summary>
    /// 是否存在当前分支
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsExsitBranchAsync(string branch)
    {
        using var client = _httpClientFactory.CreateClient();
        var response =
            await client.GetAsync(
                $"{GiteeHost}/repos/{Owner}/{Repo}/branches/{branch}?access_token={_accessToken}");
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 获取所有分支
    /// </summary>
    /// <returns></returns>
    public async Task<List<string>> GetAllBranchAsync()
    {
        using var client = _httpClientFactory.CreateClient();
        var response =
            await client.GetAsync(
                $"{GiteeHost}/repos/{Owner}/{Repo}/branches?access_token={_accessToken}&sort=name&direction=asc&page=1&per_page=100");
        response.EnsureSuccessStatusCode();
       var result= await response.Content.ReadAsStringAsync();
       JArray jsonArray=  JArray.Parse(result);
       // 创建一个列表来存储名字
       List<string> names = new List<string>();

       // 遍历每个对象，获取 name 字段
       foreach (JObject obj in jsonArray)
       {
           // 获取 name 字段的值
           string name = obj["name"]?.ToString();
           if (name != null)
           {
               names.Add(name);
           }
       }
        return names;
    }
    
    
    /// <summary>
    /// 下载仓库分支代码
    /// </summary>
    /// <param name="branch"></param>
    /// <returns></returns>
    public async Task<Stream> DownLoadFileAsync(string branch)
    {
        using var client = _httpClientFactory.CreateClient();
        var response =
            await client.GetAsync(
                $"{GiteeHost}/repos/{Owner}/{Repo}/zipball?access_token={_accessToken}&ref={branch}");
        response.EnsureSuccessStatusCode();
       return await response.Content.ReadAsStreamAsync();
       
    }
}