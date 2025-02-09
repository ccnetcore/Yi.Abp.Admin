using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Yi.Framework.Bbs.Application.Contracts.Dtos.MyType;
using Yi.Framework.Bbs.Application.Contracts.IServices;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Forum
{
    /// <summary>
    /// DiscussLable服务实现
    /// </summary>
    public class DiscussLableService : YiCrudAppService<DiscussLableAggregateRoot, DiscussLableOutputDto,
            DiscussLableGetListOutputDto, Guid, DiscussLableGetListInputVo, DiscussLableCreateInputVo,
            DiscussLableUpdateInputVo>,
        IDiscussLableService
    {
        private ISqlSugarRepository<DiscussLableAggregateRoot, Guid> _repository;

        public DiscussLableService(ISqlSugarRepository<DiscussLableAggregateRoot, Guid> repository) : base(repository)
        {
            _repository = repository;
        }
        
        [HttpGet("discuss-lable/all")]
        public async Task<ListResultDto<DiscussLableGetListOutputDto>> GetAllListAsync(DiscussLableGetListInputVo input)
        {
            var order = input.Sorting ?? nameof(DiscussLableAggregateRoot.Name);
            var output = await _repository._DbQueryable
                .WhereIF(input.Name is not null, x => x.Name.Contains(input.Name))
                .OrderBy(order)
                .Select(x => new DiscussLableGetListOutputDto(), true)
                .ToListAsync();
            return  new ListResultDto<DiscussLableGetListOutputDto>(output);
        }

        public override async Task<PagedResultDto<DiscussLableGetListOutputDto>> GetListAsync(
            DiscussLableGetListInputVo input)
        {
            RefAsync<int> total = 0;
            var order = input.Sorting ?? nameof(DiscussLableAggregateRoot.Name);
            var output = await _repository._DbQueryable
                .WhereIF(input.Name is not null, x => x.Name.Contains(input.Name))
                .OrderBy(order)
                .Select(x => new DiscussLableGetListOutputDto(), true)
                .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);
            return new PagedResultDto<DiscussLableGetListOutputDto>(total, output);
        }
    }
}