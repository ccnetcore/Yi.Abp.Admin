using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.IServices;

public interface IValueAnalyseService
{
    /// <summary>
    /// 价值排行榜
    /// </summary>
    /// <returns></returns>
    // [HttpGet("analyse/dc-user/value-top/{userId?}")]
    Task<PagedResultDto<DcValueTopUserDto>> GetValueTopAsync([FromQuery] PagedResultRequestDto input,
        [FromRoute] Guid? userId);
}