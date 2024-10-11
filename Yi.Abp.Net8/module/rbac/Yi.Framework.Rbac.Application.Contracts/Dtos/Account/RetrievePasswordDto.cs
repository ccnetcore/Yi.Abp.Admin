namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Account;

public class RetrievePasswordDto
{
    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 唯一标识码
    /// </summary>
    public string? Uuid { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public long Phone { get; set; }

    /// <summary>
    /// 验证码
    /// </summary>
    public string? Code { get; set; }
}