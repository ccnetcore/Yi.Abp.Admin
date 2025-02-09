namespace Yi.Framework.DigitalCollectibles.Domain.Dtos;

public class UserMiningLimitCacheDto
{
    /// <summary>
    /// 当前已挖矿次数
    /// </summary>
    public int Number { get; set; }
    
    /// <summary>
    /// 上次挖矿时间
    /// </summary>
    public DateTime? LastMiningTime{ get; set; }
}