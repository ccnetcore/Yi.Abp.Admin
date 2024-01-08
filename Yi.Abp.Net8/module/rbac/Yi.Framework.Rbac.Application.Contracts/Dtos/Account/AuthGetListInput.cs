using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Account
{
    public class AuthGetListInput:PagedAllResultRequestDto
    {
        public Guid? UserId { get; set; }

        public string? OpenId { get; set; }

        public string? AuthType { get; set; }
    }
}
