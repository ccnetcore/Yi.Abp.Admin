using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.InvitationCode;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Managers;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Application.Services;

/// <summary>
/// 邀请码应用服务
/// </summary>
public class InvitationCodeService : ApplicationService
{
    private readonly InvitationCodeManager _invitationCodeManager;

    public InvitationCodeService(InvitationCodeManager invitationCodeManager)
    {
        _invitationCodeManager = invitationCodeManager;
    }

    /// <summary>
    /// 查询当前登录用户的邀请码数据
    /// </summary>
    /// <returns></returns>
    [Authorize]
    public async Task<InvitationCodeGetOutputDto> GetAsync()
    {
        var userId = CurrentUser.GetId();
        var entity = await _invitationCodeManager.TryGetOrAddAsync(userId);
        var output = new InvitationCodeGetOutputDto
        {
            IsInvited = entity.IsInvited,
            PointsNumber = entity.PointsNumber,
            InvitationCode = entity.InvitationCode
        };
        return output;
    }

    /// <summary>
    /// 当前用户填写邀请码
    /// </summary>
    /// <param name="invitedUserId"></param>
    /// <exception cref="UserFriendlyException"></exception>
    [Authorize]
    [HttpPost("invitation-code/{code}")]
    public async Task SetAsync([FromRoute] string code)
    {
        var userId = CurrentUser.GetId();
        await _invitationCodeManager.SetAsync(userId, code);
    }
}