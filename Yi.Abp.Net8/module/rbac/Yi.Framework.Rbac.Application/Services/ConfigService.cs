using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Config;
using Yi.Framework.Rbac.Application.Contracts.IServices;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Rbac.Application.Services
{
    /// <summary>
    /// Config服务实现
    /// </summary>
    public class ConfigService : YiCrudAppService<ConfigAggregateRoot, ConfigGetOutputDto, ConfigGetListOutputDto, Guid, ConfigGetListInputVo, ConfigCreateInputVo, ConfigUpdateInputVo>,
       IConfigService
    {
        private ISqlSugarRepository<ConfigAggregateRoot, Guid> _repository;
        public ConfigService(ISqlSugarRepository<ConfigAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 多查
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override async Task<PagedResultDto<ConfigGetListOutputDto>> GetListAsync(ConfigGetListInputVo input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.ConfigKey), x => x.ConfigKey.Contains(input.ConfigKey!))
                          .WhereIF(!string.IsNullOrEmpty(input.ConfigName), x => x.ConfigName!.Contains(input.ConfigName!))
                          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<ConfigGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }
    }
}
