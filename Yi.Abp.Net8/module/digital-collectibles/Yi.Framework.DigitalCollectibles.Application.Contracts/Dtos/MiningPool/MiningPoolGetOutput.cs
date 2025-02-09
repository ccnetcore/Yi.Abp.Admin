namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.MiningPool;

public class MiningPoolGetOutput
{
    /// <summary>
    /// 普通藏品剩余数量
    /// </summary>
    public int I0_OrdinaryNumber { get; set; }
   
    /// <summary>
    /// 高级藏品剩余数量
    /// </summary>
    public int I1_SeniorNumber { get; set; }
   
    /// <summary>
    /// 稀有藏品剩余数量
    /// </summary>
    public int I2_RareNumber { get; set; }
   
    /// <summary>
    /// 珍品藏品剩余数量
    /// </summary>
    public int I3_GemNumber { get; set; }

    /// <summary>
    /// 传说藏品剩余数量
    /// </summary>
    public int I4_LegendNumber { get; set; }
   
    /// <summary>
    /// 开始时间
    /// </summary>
    public DateTime StartTime{ get; set; }
   
    /// <summary>
    /// 结束时间
    /// </summary>
    public DateTime EndTime{ get; set; }

    /// <summary>
    /// 总共剩余藏品数量
    /// </summary>
    public int TotalNumber => I0_OrdinaryNumber + I1_SeniorNumber + I2_RareNumber + I3_GemNumber + I4_LegendNumber;
}