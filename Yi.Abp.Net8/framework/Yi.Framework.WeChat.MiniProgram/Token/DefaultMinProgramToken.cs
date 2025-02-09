using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Yi.Framework.Core.Extensions;
using Yi.Framework.WeChat.MiniProgram.HttpModels;

namespace Yi.Framework.WeChat.MiniProgram.Token;

internal class DefaultMinProgramToken:IMiniProgramToken
{
    private const string Url = "https://api.weixin.qq.com/cgi-bin/token";
    private WeChatMiniProgramOptions _options;
    public DefaultMinProgramToken(IOptions<WeChatMiniProgramOptions> options)
    {
        _options = options.Value;
    }
    public async Task<string> GetTokenAsync()
    {
        var token = await this.GetAccessToken();
        return token.access_token;
    }
    public async Task<AccessTokenResponse> GetAccessToken()
    {
        var req = new AccessTokenRequest();
        req.appid = _options.AppID;
        req.secret = _options.AppSecret;
        req.grant_type = "client_credential";
        using (HttpClient httpClient = new HttpClient())
        {
            string queryString = req.ToQueryString();
            var builder = new UriBuilder(Url);
            builder.Query = queryString;
            HttpResponseMessage response = await httpClient.GetAsync(builder.ToString());

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            return responseBody;
        }
    }
}