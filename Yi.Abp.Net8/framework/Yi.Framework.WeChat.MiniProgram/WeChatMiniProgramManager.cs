using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Core.Extensions;
using Yi.Framework.WeChat.MiniProgram.HttpModels;
using Yi.Framework.WeChat.MiniProgram.Token;

namespace Yi.Framework.WeChat.MiniProgram;

public class WeChatMiniProgramManager : IWeChatMiniProgramManager, ISingletonDependency
{
    private IMiniProgramToken _weChatToken;
    private WeChatMiniProgramOptions _options;

    public WeChatMiniProgramManager(IMiniProgramToken weChatToken, IOptions<WeChatMiniProgramOptions> options)
    {
        _weChatToken = weChatToken;
        _options = options.Value;
    }

    /// <summary>
    /// 获取用户openid
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<Code2SessionResponse> Code2SessionAsync(Code2SessionInput input)
    {
        string url = "https://api.weixin.qq.com/sns/jscode2session";
        var req = new Code2SessionRequest();
        req.js_code = input.js_code;
        req.secret = _options.AppSecret;
        req.appid = _options.AppID;

        using (HttpClient httpClient = new HttpClient())
        {
            string queryString = req.ToQueryString();
            var builder = new UriBuilder(url);
            builder.Query = queryString;
            HttpResponseMessage response = await httpClient.GetAsync(builder.ToString());
            var responseBody = await response.Content.ReadFromJsonAsync<Code2SessionResponse>();

            responseBody.ValidateSuccess();

            return responseBody;
        }
    }


    
    /// <summary>
    /// 发送模板订阅消息
    /// </summary>
    /// <param name="input"></param>
    public async Task SendSubscribeNoticeAsync(SubscribeNoticeInput input)
    {
        var token = await _weChatToken.GetTokenAsync();
        string url = $"https://api.weixin.qq.com/cgi-bin/message/subscribe/send?access_token={token}";
        var req = new SubscribeNoticeRequest
        {
            touser = input.touser,
            template_id = input.template_id,
            page = input.page,
            data = input.data,
            miniprogram_state = _options.Notice?.State??"formal"
        };
        req.template_id=req.template_id?? _options.Notice?.TemplateId;
 
        using (HttpClient httpClient = new HttpClient())
        {
            var body =new StringContent(JsonConvert.SerializeObject(req));
            HttpResponseMessage response = await httpClient.PostAsync(url, body);
            var responseBody = await response.Content.ReadFromJsonAsync<SubscribeNoticeResponse>();
            responseBody.ValidateSuccess();
        }
    }
}