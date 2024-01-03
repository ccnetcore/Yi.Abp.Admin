using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Dept;

namespace Yi.Framework.Rbac.Application.Contracts.IServices
{
    /// <summary>
    /// Dept服务抽象
    /// </summary>
    public interface IDeptService : IYiCrudAppService<DeptGetOutputDto, DeptGetListOutputDto, Guid, DeptGetListInputVo, DeptCreateInputVo, DeptUpdateInputVo>
    {
        Task<List<Guid>> GetChildListAsync(Guid deptId);
    }
}
