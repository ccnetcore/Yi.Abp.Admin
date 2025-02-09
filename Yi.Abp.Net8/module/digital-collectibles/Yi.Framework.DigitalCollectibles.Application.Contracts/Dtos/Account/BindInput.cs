namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Account;

public class BindInput
{
    public string JsCode { get; set; }
    
    public long Phone { get; set; }
    
    /// <summary>
    /// 唯一标识码
    /// </summary>
    public string? Uuid { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }
}