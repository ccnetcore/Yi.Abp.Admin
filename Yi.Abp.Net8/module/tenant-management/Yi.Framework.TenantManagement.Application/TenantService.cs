using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;
using Yi.Framework.TenantManagement.Application.Contracts;
using Yi.Framework.TenantManagement.Application.Contracts.Dtos;
using Yi.Framework.TenantManagement.Domain;

namespace Yi.Framework.TenantManagement.Application
{
    /// <summary>
    /// 租户管理
    /// </summary>
    public class TenantService :
        YiCrudAppService<TenantAggregateRoot, TenantGetOutputDto, TenantGetListOutputDto, Guid, TenantGetListInput,
            TenantCreateInput, TenantUpdateInput>, ITenantService
    {
        private ISqlSugarRepository<TenantAggregateRoot, Guid> _repository;
        private IDataSeeder _dataSeeder;

        public TenantService(ISqlSugarRepository<TenantAggregateRoot, Guid> repository, IDataSeeder dataSeeder) :
            base(repository)
        {
            _repository = repository;
            _dataSeeder = dataSeeder;
        }

        /// <summary>
        /// 租户单查
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task<TenantGetOutputDto> GetAsync(Guid id)
        {
            return base.GetAsync(id);
        }

        /// <summary>
        /// 租户多查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<TenantGetListOutputDto>> GetListAsync(TenantGetListInput input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable
                .WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name!))
                .WhereIF(input.StartTime is not null && input.EndTime is not null,
                    x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<TenantGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }

        /// <summary>
        /// 租户选项
        /// </summary>
        /// <returns></returns>
        public async Task<List<TenantSelectOutputDto>> GetSelectAsync()
        {
            var entites = await _repository._DbQueryable.ToListAsync();
            return entites.Select(x => new TenantSelectOutputDto { Id = x.Id, Name = x.Name }).ToList();
        }


        /// <summary>
        /// 创建租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<TenantGetOutputDto> CreateAsync(TenantCreateInput input)
        {
            if (await _repository.IsAnyAsync(x => x.Name == input.Name))
            {
                throw new UserFriendlyException("创建失败，当前租户已存在");
            }

            return await base.CreateAsync(input);
        }

        /// <summary>
        /// 更新租户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<TenantGetOutputDto> UpdateAsync(Guid id, TenantUpdateInput input)
        {
            if (await _repository.IsAnyAsync(x => x.Name == input.Name && x.Id != id))
            {
                throw new UserFriendlyException("更新后租户名已经存在");
            }

            return await base.UpdateAsync(id, input);
        }


        /// <summary>
        /// 租户删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task DeleteAsync(IEnumerable<Guid> id)
        {
            return base.DeleteAsync(id);
        }


        /// <summary>
        /// 初始化租户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("tenant/init/{id}")]
        public async Task InitAsync([FromRoute] Guid id)
        {
            await CurrentUnitOfWork.SaveChangesAsync();
            using (CurrentTenant.Change(id))
            {
                await CodeFirst(this.LazyServiceProvider);
                 await _dataSeeder.SeedAsync(id);
            }
        }

        private async Task CodeFirst(IServiceProvider service)
        {
            var moduleContainer = service.GetRequiredService<IModuleContainer>();

            //没有数据库，不能创工作单元，创建库，先关闭
            using (var uow = UnitOfWorkManager.Begin(requiresNew: true, isTransactional: false))
            {
                ISqlSugarClient db = await _repository.GetDbContextAsync();
                //尝试创建数据库
                db.DbMaintenance.CreateDatabase();

                List<Type> types = new List<Type>();
                foreach (var module in moduleContainer.Modules)
                {
                    types.AddRange(module.Assembly.GetTypes()
                        .Where(x => x.GetCustomAttribute<IgnoreCodeFirstAttribute>() == null)
                        .Where(x => x.GetCustomAttribute<SugarTable>() != null)
                        .Where(x => x.GetCustomAttribute<DefaultTenantTableAttribute>() is null)
                        .Where(x => x.GetCustomAttribute<SplitTableAttribute>() is null));
                }

                if (types.Count > 0)
                {
                    db.CodeFirst.InitTables(types.ToArray());
                }

                await uow.CompleteAsync();
            }
        }
    }
}