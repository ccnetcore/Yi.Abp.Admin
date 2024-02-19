using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Yi.Framework.Ddd.Application
{
    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey> : YiCrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
         where TEntity : class, IEntity<TKey>
         where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }

    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }


    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }

    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : YiCrudAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }
    }


    public abstract class YiCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
    where TEntity : class, IEntity<TKey>
    where TGetOutputDto : IEntityDto<TKey>
    where TGetListOutputDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository) : base(repository)
        {
        }

        /// <summary>
        /// 多查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            List<TEntity>? entites = null;
            //区分多查还是批量查
            if (input is IPagedResultRequest pagedInput)
            {
                entites = await Repository.GetPagedListAsync(pagedInput.SkipCount, pagedInput.MaxResultCount, string.Empty);
            }
            else
            {
                entites = await Repository.GetListAsync();
            }
            var total = await Repository.GetCountAsync();
            var output = await MapToGetListOutputDtosAsync(entites);
            return new PagedResultDto<TGetListOutputDto>(total, output);
            //throw new NotImplementedException($"【{typeof(TEntity)}】实体的CrudAppService，查询为具体业务，通用查询几乎无实际场景，请重写实现！");
        }

        /// <summary>
        /// 多删
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RemoteService(isEnabled: true)]
        public virtual async Task DeleteAsync(IEnumerable<TKey> id)
        {
            await Repository.DeleteManyAsync(id);
        }

        /// <summary>
        /// 偷梁换柱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RemoteService(isEnabled: false)]
        public override Task DeleteAsync(TKey id)
        {
            return base.DeleteAsync(id);
        }


        /// <summary>
        /// 导出excel
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> GetExportExcelAsync(TGetListInput input)
        {
            if (input is IPagedResultRequest paged)
            {
                paged.SkipCount = 0;
                paged.MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount;
            }

            var output = await this.GetListAsync(input);
            var dirPath = $"/wwwroot/temp";

            var fileName = $"{typeof(TEntity).Name}_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}_{Guid.NewGuid()}";
            var filePath = $"{dirPath}/{fileName}.xlsx";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            MiniExcel.SaveAs(filePath, output.Items);
            return new PhysicalFileResult(filePath, "application/vnd.ms-excel");
        }

        /// <summary>
        /// 导入excle
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task PostImportExcelAsync(List<TCreateInput> input)
        {
            var entities = input.Select(x => MapToEntity(x)).ToList();
            //安全起见，该接口需要自己实现
            throw new NotImplementedException();
            //await Repository.DeleteManyAsync(entities.Select(x => x.Id));
            //await Repository.InsertManyAsync(entities);
        }
    }
}
