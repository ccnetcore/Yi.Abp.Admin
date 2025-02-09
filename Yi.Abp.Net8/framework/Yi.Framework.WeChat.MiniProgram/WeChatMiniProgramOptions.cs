namespace Yi.Framework.WeChat.MiniProgram;

public class WeChatMiniProgramOptions
{
    /// <summary>
    /// AppId
    /// </summary>
    public string AppID { get; set; }
    
    /// <summary>
    /// App密钥
    /// </summary>
    public string AppSecret { get; set; }

    /// <summary>
    /// 消息
    /// </summary>
    public WeChatMiniProgramNoticeItem Notice { get; set; }

}

public class WeChatMiniProgramNoticeItem
{
    /// <summary>
    /// 模板id
    /// </summary>
    public string TemplateId { get; set; }

    public string State { get; set; }
}