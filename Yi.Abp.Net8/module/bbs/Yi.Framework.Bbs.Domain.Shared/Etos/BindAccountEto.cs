namespace Yi.Framework.Bbs.Domain.Shared.Etos;

/// <summary>
/// 临时用户绑定到正式用户
/// </summary>
public class BindAccountEto
{
    public Guid NewUserId { get; set; }
    public Guid OldUserId { get; set; }
}