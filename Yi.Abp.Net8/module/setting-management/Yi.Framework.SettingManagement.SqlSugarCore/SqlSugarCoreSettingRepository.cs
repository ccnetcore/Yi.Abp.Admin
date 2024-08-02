using Yi.Framework.SettingManagement.Domain;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.SqlSugarCore.Repositories;

namespace Volo.Abp.SettingManagement.EntityFrameworkCore;

public class SqlSugarCoreSettingRepository : SqlSugarRepository<SettingAggregateRoot, Guid>,
    ISettingRepository
{
    public SqlSugarCoreSettingRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
    {
    }

    public virtual async Task<SettingAggregateRoot> FindAsync(
        string name,
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        return await _DbQueryable
            .Where(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .OrderBy(x => x.Id)
            .FirstAsync();
    }

    public virtual async Task<List<SettingAggregateRoot>> GetListAsync(
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        return await _DbQueryable
            .Where(
                s => s.ProviderName == providerName && s.ProviderKey == providerKey
            ).ToListAsync();
    }

    public virtual async Task<List<SettingAggregateRoot>> GetListAsync(
        string[] names,
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        return await _DbQueryable
            .Where(
                s => names.Contains(s.Name) && s.ProviderName == providerName && s.ProviderKey == providerKey
            ).ToListAsync();
    }
}
