using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.IServices;

public interface IPointAnalyseService
{
    /// <summary>
    /// 积分排行榜
    /// </summary>
    /// <returns></returns>
    // [HttpGet("analyse/dc-user/points-top/{userId?}")]
    Task<PagedResultDto<DcPointsTopUserDto>> GetValueTopAsync([FromQuery] PagedResultRequestDto input,
        [FromRoute] Guid? userId);
}