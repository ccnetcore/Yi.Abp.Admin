using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;
using Yi.Framework.DigitalCollectibles.Application.Contracts.IServices;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;


namespace Yi.Framework.DigitalCollectibles.Application.Services.Analyses;

/// <summary>
/// 用户积分分析
/// </summary>
public class PointAnalyseService: ApplicationService,IPointAnalyseService
{

    private readonly ISqlSugarRepository<InvitationCodeAggregateRoot> _repository;

    public PointAnalyseService(ISqlSugarRepository<InvitationCodeAggregateRoot> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 积分排行榜
    /// </summary>
    /// <returns></returns>
    // [HttpGet("analyse/dc-user/points-top/{userId?}")]
    [RemoteService(isEnabled:false)]
    public async Task<PagedResultDto<DcPointsTopUserDto>> GetValueTopAsync([FromQuery] PagedResultRequestDto input,
        [FromRoute] Guid? userId)
    {

        var pageIndex = input.SkipCount;
        RefAsync<int> total = 0;
        var output = await _repository._DbQueryable
            .OrderByDescending(x=>x.PointsNumber)
            .Select(x =>
            new DcPointsTopUserDto{
                UserId = x.UserId,
                Points = x.PointsNumber,
                Order=SqlFunc.RowNumber(SqlFunc.Desc(x.PointsNumber))
            })
            .ToPageListAsync(pageIndex, input.MaxResultCount, total);
        return new PagedResultDto<DcPointsTopUserDto>
        {
            Items = output,
            TotalCount = total
        };
      
    }
}