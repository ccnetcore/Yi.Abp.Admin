using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Yi.Framework.CodeGen.Application.Contracts.Dtos.Template;
using Yi.Framework.CodeGen.Application.Contracts.IServices;
using Yi.Framework.CodeGen.Domain.Entities;
using Yi.Framework.Ddd.Application;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.CodeGen.Application.Services;

public class TemplateService : YiCrudAppService<TemplateAggregateRoot, TemplateDto, Guid, TemplateGetListInput>, ITemplateService
{
    private ISqlSugarRepository<TemplateAggregateRoot, Guid> _repository;
    public TemplateService(ISqlSugarRepository<TemplateAggregateRoot, Guid> repository) : base(repository)
    {
        _repository = repository;
    }

    public async override Task<PagedResultDto<TemplateDto>> GetListAsync([FromQuery] TemplateGetListInput input)
    {
        RefAsync<int> total = 0;
        var entities = await _repository._DbQueryable.WhereIF(input.Name is not null, x => x.Name.Equals(input.Name!))
                  .ToPageListAsync(input.SkipCount, input.MaxResultCount, total);

        return new PagedResultDto<TemplateDto>
        {
            TotalCount = total,
            Items = await MapToGetListOutputDtosAsync(entities)
        };
    }
}

