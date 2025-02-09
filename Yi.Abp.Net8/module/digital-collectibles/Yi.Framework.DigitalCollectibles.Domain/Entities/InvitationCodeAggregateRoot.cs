using SqlSugar;
using Volo.Abp.Domain.Entities.Auditing;

namespace Yi.Framework.DigitalCollectibles.Domain.Entities;

[SugarTable("DC_InvitationCode")]
public class InvitationCodeAggregateRoot : FullAuditedAggregateRoot<Guid>
{
    public InvitationCodeAggregateRoot()
    {
    }
    public InvitationCodeAggregateRoot(Guid userId, string invitationCode)
    {
        this.UserId = userId;
        this.InvitationCode = invitationCode;
    }
    

    /// <summary>
    /// 谁的邀请码
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 是否填写了邀请码（是否被邀请）
    /// </summary>
    public bool IsInvited { get; set; } = false;


    /// <summary>
    /// 积分-邀请数量
    /// </summary>
    public int PointsNumber { get; set; } = 0;

    /// <summary>
    /// 邀请码
    /// </summary>
    public string InvitationCode { get; set; }

    /// <summary>
    /// 这个人填写了邀请码（不能再进行填写）
    /// </summary>
    public void SetInvite()
    {
        IsInvited = true;
      
    }

    /// <summary>
    /// 别人填写了这个用户的邀请码（这个用户积分+1）
    /// </summary>
    public void SetInvited()
    {
        PointsNumber += 1;
    }

    //不做记录
    // /// <summary>
    // /// 邀请者的id
    // /// </summary>
    // public Guid InviterUserId{ get; set; }
}