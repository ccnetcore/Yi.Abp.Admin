using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Plate;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Ddd.Application;
using Yi.Framework.Rbac.Application.Contracts.Dtos.Config;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// Plate服务实现
    /// </summary>
    public class PlateService : YiCrudAppService<PlateAggregateRoot, PlateGetOutputDto, PlateGetListOutputDto, Guid, PlateGetListInputVo, PlateCreateInputVo, PlateUpdateInputVo>,
       IPlateService
    {
        private ISqlSugarRepository<PlateAggregateRoot, Guid> _repository;
        public PlateService(ISqlSugarRepository<PlateAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }

        public override async Task<PagedResultDto<PlateGetListOutputDto>> GetListAsync(PlateGetListInputVo input)
        {
            RefAsync<int> total = 0;

            var entities = await _repository._DbQueryable.WhereIF(!string.IsNullOrEmpty(input.Name), x => x.Name.Contains(input.Name!))
                .WhereIF(!string.IsNullOrEmpty(input.Code), x => x.Name.Contains(input.Code!))
                          .WhereIF(input.StartTime is not null && input.EndTime is not null, x => x.CreationTime >= input.StartTime && x.CreationTime <= input.EndTime)
                          .OrderByDescending(x => x.OrderNum)
                          .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<PlateGetListOutputDto>(total, await MapToGetListOutputDtosAsync(entities));
        }
    }
}
