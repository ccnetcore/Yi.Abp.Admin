using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.TenantManagement.Application.Contracts.Dtos;

namespace Yi.Framework.TenantManagement.Application.Contracts
{
    public interface ITenantService:IYiCrudAppService< TenantGetOutputDto, TenantGetListOutputDto, Guid, TenantGetListInput, TenantCreateInput, TenantUpdateInput>
    {
    }
}
