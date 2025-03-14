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
    /// <summary>
    /// 缓存键标准化处理器
    /// 用于处理缓存键的格式化和多租户支持
    /// </summary>
    [Dependency(ReplaceServices = true)]
    public class YiDistributedCacheKeyNormalizer : IDistributedCacheKeyNormalizer, ITransientDependency
    {
        private readonly ICurrentTenant _currentTenant;
        private readonly AbpDistributedCacheOptions _distributedCacheOptions;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currentTenant">当前租户服务</param>
        /// <param name="distributedCacheOptions">分布式缓存配置选项</param>
        public YiDistributedCacheKeyNormalizer(
            ICurrentTenant currentTenant,
            IOptions<AbpDistributedCacheOptions> distributedCacheOptions)
        {
            _currentTenant = currentTenant;
            _distributedCacheOptions = distributedCacheOptions.Value;
        }

        /// <summary>
        /// 标准化缓存键
        /// </summary>
        /// <param name="args">缓存键标准化参数</param>
        /// <returns>标准化后的缓存键</returns>
        public virtual string NormalizeKey(DistributedCacheKeyNormalizeArgs args)
        {
            // 添加全局缓存前缀
            var normalizedKey = $"{_distributedCacheOptions.KeyPrefix}{args.Key}";

            //todo 多租户支持已注释，如需启用取消注释即可
            //if (!args.IgnoreMultiTenancy && _currentTenant.Id.HasValue)
            //{
            //    normalizedKey = $"t:{_currentTenant.Id.Value},{normalizedKey}";
            //}

            return normalizedKey;
        }
    }
}
