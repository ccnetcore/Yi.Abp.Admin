using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace Yi.Framework.TenantManagement.Domain
{
    public class SqlSugarTenantStore : ITenantStore
    {
        private ISqlSugarTenantRepository TenantRepository { get; }
        protected ICurrentTenant CurrentTenant { get; }
        protected IDistributedCache<TenantCacheItem> Cache { get; }
        public SqlSugarTenantStore(ISqlSugarTenantRepository repository,
            IDistributedCache<TenantCacheItem> cache,
        ICurrentTenant currentTenant)
        { TenantRepository = repository;
            Cache=cache;
            CurrentTenant=currentTenant;
        }

        public TenantConfiguration? Find(string name)
        {
            throw new NotImplementedException("请使用异步方法");
        }

        public TenantConfiguration? Find(Guid id)
        {
            throw new NotImplementedException("请使用异步方法");
        }

        public async Task<TenantConfiguration?> FindAsync(string name)
        {
            return (await GetCacheItemAsync(null, name)).Value;
        }

        public async Task<TenantConfiguration?> FindAsync(Guid id)
        {
            return (await GetCacheItemAsync(id, null)).Value;
        }





        protected virtual async Task<TenantCacheItem> GetCacheItemAsync(Guid? id, string name)
        {
            var cacheKey = CalculateCacheKey(id, name);

            var cacheItem = await Cache.GetAsync(cacheKey, considerUow: true);
            if (cacheItem != null)
            {
                return cacheItem;
            }

            if (id.HasValue)
            {
                using (CurrentTenant.Change(null)) //TODO: No need this if we can implement to define host side (or tenant-independent) entities!
                {
                    var tenant = await TenantRepository.FindAsync(id.Value);
                    return await SetCacheAsync(cacheKey, tenant);
                }
            }

            if (!name.IsNullOrWhiteSpace())
            {
                using (CurrentTenant.Change(null)) //TODO: No need this if we can implement to define host side (or tenant-independent) entities!
                {
                    var tenant = await TenantRepository.FindByNameAsync(name);
                    return await SetCacheAsync(cacheKey, tenant);
                }
            }
            throw new AbpException("Both id and name can't be invalid.");
        }

        protected virtual async Task<TenantCacheItem> SetCacheAsync(string cacheKey, [CanBeNull] TenantAggregateRoot tenant)
        {
            var tenantConfiguration = tenant != null ? MapToConfiguration(tenant) : null;
            var cacheItem = new TenantCacheItem(tenantConfiguration);
            await Cache.SetAsync(cacheKey, cacheItem, considerUow: true);
            return cacheItem;
        }

        private TenantConfiguration MapToConfiguration(TenantAggregateRoot tenantAggregateRoot)
        {
            var tenantConfiguration = new TenantConfiguration();
            tenantConfiguration.Id = tenantAggregateRoot.Id;
            tenantConfiguration.Name = tenantAggregateRoot.Name;
            tenantConfiguration.ConnectionStrings = MaptoString(tenantAggregateRoot.TenantConnectionString);
            tenantConfiguration.IsActive = true;
            return tenantConfiguration;
        }

        private ConnectionStrings? MaptoString(string tenantConnectionString)
        {

            //tenantConnectionString = tenantConnectionString.TrimEnd(';');
            //var strSpiteds = tenantConnectionString.Split(";");
            //if (strSpiteds.Count() == 0)
            //{
            //    return null;

            //}

            var connectionStrings = new ConnectionStrings();
            //foreach (string strSpited in strSpiteds)
            //{
            //    var key = strSpited.Split('=')[0];
            //    var value = strSpited.Split('=')[1];
            //    connectionStrings[key] = value;
            //}
            connectionStrings["test"] = tenantConnectionString;

            return connectionStrings;
        }

        protected virtual string CalculateCacheKey(Guid? id, string name)
        {
            return TenantCacheItem.CalculateCacheKey(id, name);
        }
    }
}
