namespace Yi.Framework.Bbs.Domain.Shared.Etos;

public class SetAccountInfoEto
{
    public SetAccountInfoEto(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; set; }
    
    
    /// <summary>
    /// 钱钱
    /// </summary>
    public decimal Money { get; set; }
    
    /// <summary>
    /// 积分
    /// </summary>
    public int Points { get; set; }
    
    /// <summary>
    /// 价值
    /// </summary>
    public decimal Value { get; set; }
}