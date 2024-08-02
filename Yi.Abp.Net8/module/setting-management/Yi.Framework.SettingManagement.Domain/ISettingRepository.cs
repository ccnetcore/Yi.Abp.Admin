using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Yi.Framework.SettingManagement.Domain;

public interface ISettingRepository : IBasicRepository<SettingAggregateRoot, Guid>
{
    Task<SettingAggregateRoot> FindAsync(
        string name,
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default);

    Task<List<SettingAggregateRoot>> GetListAsync(
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default);

    Task<List<SettingAggregateRoot>> GetListAsync(
        string[] names,
        string providerName,
        string providerKey,
        CancellationToken cancellationToken = default);
}
