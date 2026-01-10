using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Domain.Shared.Dtos;

public class MenuTreeDto: TreeHelper.ITreeModel<MenuTreeDto>
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public int OrderNum { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public MenuTypeEnum MenuType { get; set; } = MenuTypeEnum.Menu;
    public string? MenuIcon { get; set; }
    
    public List<MenuTreeDto>? Children { get; set; }
}