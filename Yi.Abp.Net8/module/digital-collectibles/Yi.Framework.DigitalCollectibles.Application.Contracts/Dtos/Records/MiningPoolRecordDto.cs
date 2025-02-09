using Volo.Abp.Application.Dtos;
using Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Records;

public class MiningPoolRecordDto:EntityDto<Guid>
{
    /// <summary>
    /// 用户id
    /// </summary>
    public Guid UserId { get; set; }
    
    public  DateTime CreationTime { get; set; }
    
    /// <summary>
    /// 挖到的藏品
    /// </summary>
    public CollectiblesDto Collectibles { get; set; }
}