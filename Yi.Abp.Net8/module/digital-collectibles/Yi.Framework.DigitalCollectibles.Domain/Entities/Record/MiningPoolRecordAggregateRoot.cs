using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities.Record;

/// <summary>
/// 挖矿记录
/// </summary>
[SugarTable("DC_MiningPoolRecord")]
public class MiningPoolRecordAggregateRoot:FullAuditedAggregateRoot<Guid>
{
    public MiningPoolRecordAggregateRoot(Guid userId, Guid collectiblesId)
    {
        UserId = userId;
        CollectiblesId = collectiblesId;
    }

    public MiningPoolRecordAggregateRoot()
    {
    }


    /// <summary>
    /// 用户id
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// 挖到的藏品id
    /// </summary>
    public Guid CollectiblesId { get; set; }
}