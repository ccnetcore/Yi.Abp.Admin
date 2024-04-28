using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;

namespace Yi.Framework.Ddd.Application
{
    public abstract class YiCacheCrudAppService<TEntity, TEntityDto, TKey> : YiCrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
          where TEntity : class, IEntity<TKey>
          where TEntityDto : IEntityDto<TKey>
    {
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
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


    public abstract class YiCacheCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : YiCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, IEntity<TKey>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    {
        protected IDistributedCache<TEntity> Cache => LazyServiceProvider.LazyGetRequiredService<IDistributedCache<TEntity>>();

        protected string GetCacheKey(TKey id) => typeof(TEntity).Name + ":" + CurrentTenant.Id ?? Guid.Empty + ":" + id.ToString();
        protected YiCacheCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }

        public override async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input) 
        {
            var output = await base.UpdateAsync(id, input);
            await Cache.RemoveAsync(GetCacheKey(id));
            return output;
        }

        public override async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            //两种方式：
            //1：全表缓存，使用缓存直接查询
            //2：非全部缓存，查询到的数据直接添加到缓存

            //判断是否该实体为全表缓存
            throw new NotImplementedException();

            //IDistributedCache 有局限性，条件查询无法进行缓存了
            //if (true)
            //{
            //    return await GetListByCacheAsync(input);
            //}
            //else
            //{
            //    return await GetListByDbAsync(input);
            //}

        }

        protected virtual async Task<PagedResultDto<TGetListOutputDto>> GetListByDbAsync(TGetListInput input)
        {
            //如果不是全表缓存，可以走这个啦
            throw new NotImplementedException();
        }
        protected virtual async Task<PagedResultDto<TGetListOutputDto>> GetListByCacheAsync(TGetListInput input)
        {
            //如果是全表缓存，可以走这个啦
            throw new NotImplementedException();
        }


        protected override async Task<TEntity> GetEntityByIdAsync(TKey id)
        {
            var output = await Cache.GetOrAddAsync(GetCacheKey(id), async () => await base.GetEntityByIdAsync(id));
            return output!;
        }

        public override async Task DeleteAsync(IEnumerable<TKey> id)
        {
            await base.DeleteAsync(id);
            foreach (var itemId in id)
            {
                await Cache.RemoveAsync(GetCacheKey(itemId));
            }

        }
    }
}