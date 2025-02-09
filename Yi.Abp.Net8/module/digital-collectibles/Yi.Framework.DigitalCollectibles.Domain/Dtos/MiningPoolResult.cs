using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Enums;

namespace Yi.Framework.DigitalCollectibles.Domain.Dtos;

public class MiningPoolResult
{
    public CollectiblesAggregateRoot? Collectibles { get; set; }
}