using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Config;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// Config服务抽象
    /// </summary>
    public interface IConfigService : IYiCrudAppService<ConfigGetOutputDto, ConfigGetListOutputDto, Guid, ConfigGetListInputVo, ConfigCreateInputVo, ConfigUpdateInputVo>
    {

    }
}
