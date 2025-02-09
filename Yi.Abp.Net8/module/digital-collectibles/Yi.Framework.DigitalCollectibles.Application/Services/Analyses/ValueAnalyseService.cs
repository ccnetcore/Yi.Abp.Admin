using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;
using Yi.Framework.DigitalCollectibles.Application.Contracts.IServices;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Managers;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Application.Services.Analyses;

public class ValueAnalyseService: ApplicationService,IValueAnalyseService
{

    private readonly CollectiblesManager _manager;

    public ValueAnalyseService( CollectiblesManager manager)
    {
        _manager = manager;
    }

    /// <summary>
    /// 价值排行榜
    /// </summary>
    /// <returns></returns>
    // [HttpGet("analyse/dc-user/value-top/{userId?}")]
    [RemoteService(isEnabled:false)]
    public async Task<PagedResultDto<DcValueTopUserDto>> GetValueTopAsync([FromQuery] PagedResultRequestDto input,
        [FromRoute] Guid? userId)
    {
        //每个人的价值需要进行计算才能获取，这里计算时间较长，放入缓存，绝对过期
          var allValue= await _manager.GetAllAccountValueByCacheAsync();
          var output = allValue.OrderByDescending(x => x.Value).Select((x, index) => new DcValueTopUserDto
              {
                  UserId = x.UserId,
                  Value = x.Value,
                  Order = index + 1
              }).Skip((input.SkipCount - 1) * input.MaxResultCount) // 跳过前面（当前页码 - 1）* 每页数量条记录
              .Take(input.MaxResultCount).ToList();     
            return new PagedResultDto<DcValueTopUserDto>
            {
                Items = output,
                TotalCount = allValue.Count
            };
      
    }
}