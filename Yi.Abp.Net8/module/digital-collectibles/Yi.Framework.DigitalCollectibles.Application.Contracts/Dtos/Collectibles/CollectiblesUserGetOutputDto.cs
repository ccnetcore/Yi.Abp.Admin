using Volo.Abp.Application.Dtos;

namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Collectibles;

public class CollectiblesUserGetOutputDto : EntityDto<Guid>
{
    /// <summary>
    /// 藏品
    /// </summary>
    public CollectiblesDto Collectibles{ get; set; }
    
    /// <summary>
    /// 数量
    /// </summary>
    public int Number { get; set; }
}