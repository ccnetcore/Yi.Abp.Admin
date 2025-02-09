using Yi.Framework.DigitalCollectibles.Domain.Shared.Enums;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Account;

public class LoginOutput
{
    /// <summary>
    /// 后端访问token
    /// </summary>
    public string? Token { get; set; }
}