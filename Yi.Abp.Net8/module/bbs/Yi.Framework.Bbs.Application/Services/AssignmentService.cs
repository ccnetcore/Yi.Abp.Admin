using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Volo.Abp.Users;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Assignment;
using Yi.Framework.Bbs.Domain.Entities.Assignment;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Services;

/// <summary>
/// 任务系统
/// </summary>
[Authorize]
public class AssignmentService : ApplicationService
{
    private readonly AssignmentManager _assignmentManager;

    public AssignmentService(AssignmentManager assignmentManager)
    {
        _assignmentManager = assignmentManager;
    }

    /// <summary>
    /// 接收任务
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("assignment/accept/{id}")]
    public async Task AcceptAsync([FromRoute] Guid id)
    {
        await _assignmentManager.AcceptAsync(CurrentUser.GetId(), id);
    }

    /// <summary>
    /// 领取任务奖励
    /// </summary>
    /// <param name="id"></param>
    [HttpPost("assignment/complete/{id}")]
    public async Task ReceiveRewardsAsync([FromRoute] Guid id)
    {
        await _assignmentManager.ReceiveRewardsAsync(id);
    }

    /// <summary>
    /// 查看可接受的任务
    /// </summary>
    /// <returns></returns>
    [HttpGet("assignment/receive")]
    public async Task<List<AssignmentDefineGetListOutputDto>> GetCanReceiveListAsync()
    {
        var entities = await _assignmentManager.GetCanReceiveListAsync(CurrentUser.GetId());
        var output = entities.Adapt<List<AssignmentDefineGetListOutputDto>>();
        return output;
    }

    /// <summary>
    /// 查询接受的任务
    /// </summary>
    [HttpGet("assignment")]
    public async Task<List<AssignmentGetListOutputDto>> GetListAsync([FromQuery] AssignmentGetListInput input)
    {
        var output = await _assignmentManager._assignmentRepository._DbQueryable
            .Where(x => x.UserId == CurrentUser.GetId())
            .WhereIF(input.AssignmentQueryState == AssignmentQueryStateEnum.Progress,
                x => x.AssignmentState == AssignmentStateEnum.Progress||
                     x.AssignmentState == AssignmentStateEnum.Completed)
            .WhereIF(input.AssignmentQueryState == AssignmentQueryStateEnum.End,
                x => x.AssignmentState == AssignmentStateEnum.End ||
                     x.AssignmentState == AssignmentStateEnum.Expired)
            .OrderBy(x=>x.CreationTime)
            .LeftJoin<AssignmentDefineAggregateRoot>((x, define) => x.AssignmentDefineId==define.Id)
            .Select((x, define) => new AssignmentGetListOutputDto
            {
                Id = x.Id
            },true)
            .ToListAsync();

        return output;
    }
}