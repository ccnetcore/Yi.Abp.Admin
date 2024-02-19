using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Yi.Framework.Caching.FreeRedis
{
    [Dependency(ReplaceServices =true)]
    public class YiDistributedCacheKeyNormalizer : IDistributedCacheKeyNormalizer, ITransientDependency
    {
        protected ICurrentTenant CurrentTenant { get; }

        protected AbpDistributedCacheOptions DistributedCacheOptions { get; }

        public YiDistributedCacheKeyNormalizer(
            ICurrentTenant currentTenant,
            IOptions<AbpDistributedCacheOptions> distributedCacheOptions)
        {
            CurrentTenant = currentTenant;
            DistributedCacheOptions = distributedCacheOptions.Value;
        }

        public virtual string NormalizeKey(DistributedCacheKeyNormalizeArgs args)
        {
            var normalizedKey = $"{DistributedCacheOptions.KeyPrefix}{args.Key}";

            //if (!args.IgnoreMultiTenancy && CurrentTenant.Id.HasValue)
            //{
            //    normalizedKey = $"t:{CurrentTenant.Id.Value},{normalizedKey}";
            //}

            return normalizedKey;
        }
    }
}
