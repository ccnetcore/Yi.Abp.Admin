using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities;

/// <summary>
/// 挂机表
/// 表示用户与挂机道具之间的关系
/// 用于定时任务处理自动挖矿
/// </summary>
[SugarTable("DC_OnHook")]
public class OnHookAggregateRoot : FullAuditedAggregateRoot<Guid>
{
    public OnHookAggregateRoot()
    {
    }

    public OnHookAggregateRoot(Guid userId, int effectiveHours)
    {
        UserId = userId;
        EffectiveHours = effectiveHours;
        StarTime = DateTime.Now;
        EndTime = DateTime.Now.AddHours(effectiveHours);
        IsActive = true;
    }


    /// <summary>
    /// 用户id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime? StarTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 有效小时数
    /// </summary>
    public int EffectiveHours { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive { get; set; }
}