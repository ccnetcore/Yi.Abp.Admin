namespace Yi.Framework.WeChat.MiniProgram.HttpModels;

public class AccessTokenResponse
{
    public string access_token { get;  set; }

    public int expires_in { get; set; }
}

public class AccessTokenRequest
{
    public string grant_type { get; set; }
    public string appid { get; set; }
    public string secret { get; set; }
}