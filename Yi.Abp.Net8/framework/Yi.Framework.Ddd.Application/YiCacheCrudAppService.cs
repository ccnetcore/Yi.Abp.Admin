using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Yi.Framework.Ddd.Application
{
    /// <summary>
    /// 带缓存的CRUD应用服务基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TEntityDto">实体DTO类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public abstract class YiCacheCrudAppService<TEntity, TEntityDto, TKey> 
        : YiCrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) 
            : base(repository)
        {
        }
    }

    public abstract class YiCacheCrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }


    public abstract class YiCacheCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }

    public abstract class YiCacheCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : YiCrudAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }


    /// <summary>
    /// 完整的带缓存CRUD应用服务实现
    /// </summary>
    public abstract class YiCacheCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : YiCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        /// <summary>
        /// 分布式缓存访问器
        /// </summary>
        private IDistributedCache<TEntity> EntityCache => 
            LazyServiceProvider.LazyGetRequiredService<IDistributedCache<TEntity>>();

        /// <summary>
        /// 获取缓存键
        /// </summary>
        protected virtual string GenerateCacheKey(TKey id) => 
            $"{typeof(TEntity).Name}:{CurrentTenant.Id ?? Guid.Empty}:{id}";

        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) 
            : base(repository)
        {
        }

        /// <summary>
        /// 更新实体并清除缓存
        /// </summary>
        public override async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            var result = await base.UpdateAsync(id, input);
            await EntityCache.RemoveAsync(GenerateCacheKey(id));
            return result;
        }

        /// <summary>
        /// 获取实体列表(需要继承实现具体的缓存策略)
        /// </summary>
        public override Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            // 建议实现两种缓存策略:
            // 1. 全表缓存: 适用于数据量小且变动不频繁的场景
            // 2. 按需缓存: 仅缓存常用数据,适用于大数据量场景
            throw new NotImplementedException("请实现具体的缓存查询策略");
        }

        /// <summary>
        /// 从数据库获取实体列表
        /// </summary>
        protected virtual Task<PagedResultDto<TGetListOutputDto>> GetListFromDatabaseAsync(
            TGetListInput input)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 从缓存获取实体列表
        /// </summary>
        protected virtual Task<PagedResultDto<TGetListOutputDto>> GetListFromCacheAsync(
            TGetListInput input)
        {
            throw new NotImplementedException(); 
        }

        /// <summary>
        /// 获取单个实体(优先从缓存获取)
        /// </summary>
        protected override async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            return (await EntityCache.GetOrAddAsync(
                GenerateCacheKey(id),
                async () => await base.GetEntityByIdAsync(id)))!;
        }

        /// <summary>
        /// 批量删除实体并清除缓存
        /// </summary>
        public override async Task DeleteAsync(IEnumerable<TKey> ids)
        {
            await base.DeleteAsync(ids);
            
            // 批量清除缓存
            var tasks = ids.Select(id => 
                EntityCache.RemoveAsync(GenerateCacheKey(id)));
            await Task.WhenAll(tasks);
        }
    }
}