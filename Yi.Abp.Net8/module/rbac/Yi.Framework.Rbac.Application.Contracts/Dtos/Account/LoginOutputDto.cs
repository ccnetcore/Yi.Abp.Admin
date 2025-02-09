namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Account;

public class LoginOutputDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
}