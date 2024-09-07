using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser;

public class MoneyTopUserDto
{
    public string UserName { get; set; }
    public string? Nick { get; set; }
    public decimal Money { get; set; }
    public int Order { get; set; }
    public string? Icon { get; set; }
    public int Level { get; set; }
    /// <summary>
    /// 用户等级名称
    /// </summary>
    public string LevelName { get; set; }
    /// <summary>
    /// 用户限制
    /// </summary>
    public UserLimitEnum UserLimit { get; set; }
    
}