using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Repositories;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Yi.Framework.Rbac.SqlSugarCore.Repositories
{
    public class DeptRepository : SqlSugarRepository<DeptAggregateRoot, Guid>, IDeptRepository, ITransientDependency
    {
        public DeptRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
        {
        }

        public async Task<List<Guid>> GetChildListAsync(Guid deptId)
        {
            var entities = await _DbQueryable.ToChildListAsync(x => x.ParentId, deptId);
            return entities.Select(x => x.Id).ToList();
        }
        public async Task<List<DeptAggregateRoot>> GetListRoleIdAsync(Guid roleId)
        {

            return await _DbQueryable.Where(d => SqlFunc.Subqueryable<RoleDeptEntity>().Where(rd => rd.RoleId == roleId && d.Id == rd.DeptId).Any()).ToListAsync();
        }
    }
}
