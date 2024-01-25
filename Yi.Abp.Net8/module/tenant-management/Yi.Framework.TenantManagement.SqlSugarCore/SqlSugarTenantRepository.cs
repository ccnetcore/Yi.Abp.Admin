using Volo.Abp.DependencyInjection;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;
using Yi.Framework.TenantManagement.Domain;

namespace Yi.Framework.TenantManagement.SqlSugarCore
{
    public class SqlSugarTenantRepository : SqlSugarRepository<TenantAggregateRoot, Guid>, ISqlSugarTenantRepository,ITransientDependency
    {
        public SqlSugarTenantRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
        {
        }

        public async Task<TenantAggregateRoot> FindByNameAsync(string name, bool includeDetails = true)
        {
            return await _DbQueryable.FirstAsync(x => x.Name == name);
        }

        public async Task<long> GetCountAsync(string filter = null)
        {
            return await _DbQueryable.WhereIF(!string.IsNullOrEmpty(filter),x=>x.Name.Contains(filter)) .CountAsync();
        }

        public async Task<List<TenantAggregateRoot>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, string filter = null, bool includeDetails = false)
        {


            return await _DbQueryable.WhereIF(!string.IsNullOrEmpty(filter), x => x.Name.Contains(filter))
                .OrderByIF(!string.IsNullOrEmpty(sorting), sorting)
                .ToPageListAsync(skipCount, maxResultCount);
        }
    }
}
