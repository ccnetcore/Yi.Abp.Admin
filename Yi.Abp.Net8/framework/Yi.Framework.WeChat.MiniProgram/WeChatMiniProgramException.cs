namespace Yi.Framework.WeChat.MiniProgram;

public class WeChatMiniProgramException: Exception
{
    public override string Message
    {
        get
        {
            // 加上前缀
            return "微信Api异常: " + base.Message;
        }
    }

    public WeChatMiniProgramException()
    {
    }

    public WeChatMiniProgramException(string message)
        : base(message)
    {
    }

    public WeChatMiniProgramException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}