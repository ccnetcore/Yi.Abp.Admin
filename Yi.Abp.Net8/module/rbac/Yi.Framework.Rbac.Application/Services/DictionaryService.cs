using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Dictionary;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;


namespace Yi.Framework.Rbac.Application.Services
{
    /// <summary>
    /// Dictionary服务实现
    /// </summary>
    public class DictionaryService : YiCrudAppService<DictionaryEntity, DictionaryGetOutputDto, DictionaryGetListOutputDto, Guid, DictionaryGetListInputVo, DictionaryCreateInputVo, DictionaryUpdateInputVo>,
       IDictionaryService
    {
        private ISqlSugarRepository<DictionaryEntity, Guid> _repository;
        public DictionaryService(ISqlSugarRepository<DictionaryEntity, Guid> repository) : base(repository)
        {
            _repository= repository;
        }

        /// <summary>
        /// 查询
        /// </summary>

        public override async Task<PagedResultDto<DictionaryGetListOutputDto>> GetListAsync(DictionaryGetListInputVo input)
        {
            RefAsync<int> total = 0;
            var entities = await _repository._DbQueryable.WhereIF(input.DictType is not null, x => x.DictType == input.DictType)
                      .WhereIF(input.DictLabel is not null, x => x.DictLabel!.Contains(input.DictLabel!))
                      .WhereIF(input.State is not null, x => x.State == input.State)
                      .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<DictionaryGetListOutputDto>
            {
                TotalCount = total,
                Items = await MapToGetListOutputDtosAsync(entities)
            };
        }


        /// <summary>
        /// 根据字典类型获取字典列表
        /// </summary>
        /// <param name="dicType"></param>
        /// <returns></returns>
        [Route("dictionary/dic-type/{dicType}")]
        public async Task<List<DictionaryGetListOutputDto>> GetDicType([FromRoute] string dicType)
        {
            var entities = await _repository.GetListAsync(u => u.DictType == dicType && u.State == true);
            var result = await MapToGetListOutputDtosAsync(entities);
            return result;
        }
    }
}
