using Volo.Abp.Application.Dtos;
using Yi.Framework.Rbac.Domain.Shared.Enums;

namespace Yi.Framework.Rbac.Application.Contracts.Dtos.Menu
{
    public class MenuGetListInputVo : PagedAndSortedResultRequestDto
    {

        public bool? State { get; set; }
        public string? MenuName { get; set; }
        public MenuSourceEnum MenuSource { get; set; } = MenuSourceEnum.Ruoyi;
    }
}
