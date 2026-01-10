using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Yi.Framework.Ddd.Application
{
    /// <summary>
    /// CRUD应用服务基类 - 基础版本
    /// </summary>
    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey>
        : YiCrudAppService<TEntity, TEntityDto, TKey, PagedAndSortedResultRequestDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {
        }
    }

    /// <summary>
    /// CRUD应用服务基类 - 支持自定义查询输入
    /// </summary>
    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TEntityDto>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {
        }
    }

    /// <summary>
    /// CRUD应用服务基类 - 支持自定义创建输入
    /// </summary>
    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput>
        : YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TCreateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {
        }
    }

    /// <summary>
    /// CRUD应用服务基类 - 支持自定义更新输入
    /// </summary>
    public abstract class YiCrudAppService<TEntity, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : YiCrudAppService<TEntity, TEntityDto, TEntityDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TEntityDto : IEntityDto<TKey>
    {
        protected YiCrudAppService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {
        }
    }

    /// <summary>
    /// CRUD应用服务基类 - 完整实现
    /// </summary>
    public abstract class YiCrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        : CrudAppService<TEntity, TGetOutputDto, TGetListOutputDto, TKey, TGetListInput, TCreateInput, TUpdateInput>
        where TEntity : class, IEntity<TKey>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        /// <summary>
        /// 临时文件存储路径
        /// </summary>
        private const string TempFilePath = "/wwwroot/temp";

        protected YiCrudAppService(IRepository<TEntity, TKey> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="input">更新输入</param>
        /// <returns>更新后的实体DTO</returns>
        public override async Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input)
        {
            // 检查更新权限
            await CheckUpdatePolicyAsync();

            // 获取并验证实体
            var entity = await GetEntityByIdAsync(id);

            // 检查更新输入
            await CheckUpdateInputDtoAsync(entity, input);

            // 映射并更新实体
            await MapToEntityAsync(input, entity);
            await Repository.UpdateAsync(entity, autoSave: true);

            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 检查更新输入数据的有效性
        /// </summary>
        protected virtual Task CheckUpdateInputDtoAsync(TEntity entity, TUpdateInput input)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="input">创建输入</param>
        /// <returns>创建后的实体DTO</returns>
        public override async Task<TGetOutputDto> CreateAsync(TCreateInput input)
        {
            // 检查创建权限
            await CheckCreatePolicyAsync();

            // 检查创建输入
            await CheckCreateInputDtoAsync(input);

            // 映射到实体
            var entity = await MapToEntityAsync(input);

            // 设置租户ID
            TryToSetTenantId(entity);

            // 插入实体
            await Repository.InsertAsync(entity, autoSave: true);

            return await MapToGetOutputDtoAsync(entity);
        }

        /// <summary>
        /// 检查创建输入数据的有效性
        /// </summary>
        protected virtual Task CheckCreateInputDtoAsync(TCreateInput input)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="input">查询输入</param>
        /// <returns>分页结果</returns>
        public override async Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input)
        {
            List<TEntity> entities;

            // 根据输入类型决定查询方式
            if (input is IPagedResultRequest pagedInput)
            {
                // 分页查询
                entities = await Repository.GetPagedListAsync(
                    pagedInput.SkipCount,
                    pagedInput.MaxResultCount,
                    string.Empty
                );
            }
            else
            {
                // 查询全部
                entities = await Repository.GetListAsync();
            }

            // 获取总数并映射结果
            var totalCount = await Repository.GetCountAsync();
            var dtos = await MapToGetListOutputDtosAsync(entities);

            return new PagedResultDto<TGetListOutputDto>(totalCount, dtos);
        }

        /// <summary>
        /// 获取实体动态下拉框列表，子类重写该方法，通过 keywords 进行筛选
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <returns></returns>
        public virtual async Task<List<TGetListOutputDto>> GetSelectDataListAsync(string? keywords = null)
        {
            List<TEntity> entities = await Repository.GetListAsync();
            // 获取总数并映射结果
            var dtos = await MapToGetListOutputDtosAsync(entities);

            return dtos;
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="ids">实体ID集合</param>
        [RemoteService(isEnabled: true)]
        public virtual async Task DeleteAsync(IEnumerable<TKey> ids)
        {
            await Repository.DeleteManyAsync(ids);
        }

        /// <summary>
        /// 单个删除实体(禁用远程访问)
        /// </summary>
        [RemoteService(isEnabled: false)]
        public override Task DeleteAsync(TKey id)
        {
            return base.DeleteAsync(id);
        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>Excel文件</returns>
        public virtual async Task<IActionResult> GetExportExcelAsync(TGetListInput input)
        {
            // 重置分页参数以获取全部数据
            if (input is IPagedResultRequest paged)
            {
                paged.SkipCount = 0;
                paged.MaxResultCount = LimitedResultRequestDto.MaxMaxResultCount;
            }

            // 获取数据
            var output = await GetListAsync(input);

            // 确保临时目录存在
            if (!Directory.Exists(TempFilePath))
            {
                Directory.CreateDirectory(TempFilePath);
            }

            // 生成文件名和路径
            var fileName = GenerateExcelFileName();
            var filePath = Path.Combine(TempFilePath, fileName);

            // 保存Excel文件
            await MiniExcel.SaveAsAsync(filePath, output.Items);

            return new PhysicalFileResult(filePath, "application/vnd.ms-excel");
        }

        /// <summary>
        /// 生成Excel文件名
        /// </summary>
        private string GenerateExcelFileName()
        {
            return $"{typeof(TEntity).Name}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{Guid.NewGuid()}.xlsx";
        }

        /// <summary>
        /// 导入Excel(需要实现类重写此方法)
        /// </summary>
        public virtual Task PostImportExcelAsync(List<TCreateInput> input)
        {
            throw new NotImplementedException("请在实现类中重写此方法以支持Excel导入");
        }
    }
}