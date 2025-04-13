using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.Bbs.Domain.Entities.Forum;

[SugarTable("DiscussReward")]
[SugarIndex($"index_{nameof(DiscussId)}", nameof(DiscussId), OrderByType.Asc)]
public class DiscussRewardAggregateRoot : FullAuditedAggregateRoot<Guid>
{
    public Guid DiscussId { get; set; }
    
    /// <summary>
    /// 是否已解决
    /// </summary>
    public bool IsResolved{ get; set; }

    /// <summary>
    /// 悬赏最小价值
    /// </summary>
    public decimal? MinValue { get; set; }
    
    /// <summary>
    /// 悬赏最大价值
    /// </summary>
    public decimal? MaxValue { get; set; }
    
    /// <summary>
    /// 作者联系方式
    /// </summary>
    public string Contact { get; set; }

    public void SetResolved()
    {
        IsResolved = true;
    }
}