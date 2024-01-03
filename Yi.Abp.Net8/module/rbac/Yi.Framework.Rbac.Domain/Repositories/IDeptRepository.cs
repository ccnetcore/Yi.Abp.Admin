using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Domain.Repositories
{
    public interface IDeptRepository : ISqlSugarRepository<DeptEntity, Guid>
    {
        Task<List<Guid>> GetChildListAsync(Guid deptId);
        Task<List<DeptEntity>> GetListRoleIdAsync(Guid roleId);
    }
}
