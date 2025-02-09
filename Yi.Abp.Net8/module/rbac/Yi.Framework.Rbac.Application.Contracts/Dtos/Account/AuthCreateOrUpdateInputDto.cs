namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Account;

public class AuthCreateOrUpdateInputDto
{
    public Guid UserId { get; set; }

    public string OpenId { get; set; }

    public string Name { get; set; }

    public string AuthType { get; set; }
}