namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.InvitationCode;

public class InvitationCodeGetOutputDto
{
    /// <summary>
    /// 是否填写了邀请码（是否被邀请）
    /// </summary>
    public bool IsInvited { get; set; } 


    /// <summary>
    /// 积分-邀请数量
    /// </summary>
    public int PointsNumber { get; set; } 

    /// <summary>
    /// 邀请码
    /// </summary>
    public string InvitationCode { get; set; }
}