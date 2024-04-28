using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.DictionaryType;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services
{
    /// <summary>
    /// DictionaryType服务实现
    /// </summary>
    public class DictionaryTypeService : YiCrudAppService<DictionaryTypeAggregateRoot, DictionaryTypeGetOutputDto, DictionaryTypeGetListOutputDto, Guid, DictionaryTypeGetListInputVo, DictionaryTypeCreateInputVo, DictionaryTypeUpdateInputVo>,
       IDictionaryTypeService
    {
        private ISqlSugarRepository<DictionaryTypeAggregateRoot, Guid> _repository;
        public DictionaryTypeService(ISqlSugarRepository<DictionaryTypeAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public async override Task<PagedResultDto<DictionaryTypeGetListOutputDto>> GetListAsync(DictionaryTypeGetListInputVo input)
        {

            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable.WhereIF(input.DictName is not null, x => x.DictName.Contains(input.DictName!))
                      .WhereIF(input.DictType is not null, x => x.DictType!.Contains(input.DictType!))
                      .WhereIF(input.State is not null, x => x.State == input.State)
                      .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                      .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

            return new PagedResultDto<DictionaryTypeGetListOutputDto>
            {
                TotalCount = total,
                Items = await MapToGetListOutputDtosAsync(entities)
            };
        }
    }
}
