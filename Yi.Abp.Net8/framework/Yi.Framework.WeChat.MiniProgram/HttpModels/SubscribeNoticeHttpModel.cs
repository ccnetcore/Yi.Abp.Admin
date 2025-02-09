using Yi.Framework.WeChat.MiniProgram.Abstract;

namespace Yi.Framework.WeChat.MiniProgram.HttpModels;

public class SubscribeNoticeRequest
{
    /// <summary>
    ///用户openid，可以是小程序的openid，也可以是mp_template_msg.appid对应的公众号的openid
    /// </summary>
    public string touser { get; set; }

    /// <summary>
    /// 小程序模板id
    /// </summary>
    public string template_id { get; set; }
    
    /// <summary>
    /// 跳转页面
    /// </summary>
    public string page { get; set; }

    /// <summary>
    /// 小程序模板消息的数据
    /// </summary>
    public Dictionary<string, keyValueItem> data { get; set; }

    /// <summary>
    /// 默认为正式版
    /// </summary>
    public string miniprogram_state { get; set; } = "formal";

    /// <summary>
    /// 默认为中文
    /// </summary>
    public string lang { get; set; } = "zh_CN";
}


public class SubscribeNoticeInput
{
    /// <summary>
    ///用户openid，可以是小程序的openid，也可以是mp_template_msg.appid对应的公众号的openid
    /// </summary>
    public string touser { get; set; }

    /// <summary>
    /// 小程序模板id
    /// </summary>
    public string template_id { get; set; }

    /// <summary>
    /// 跳转页面
    /// </summary>
    public string page { get; set; }

    /// <summary>
    /// 公众号模板消息的数据
    /// </summary>
    public Dictionary<string, keyValueItem> data { get; set; }
}

public class SubscribeNoticeResponse : IErrorObjct
{
    public int errcode { get; set; }
    public string errmsg { get; set; }
}




public class keyValueItem
{
    public keyValueItem(string value)
    {
        this.value = value;
    }

    public string value { get; set; }
}