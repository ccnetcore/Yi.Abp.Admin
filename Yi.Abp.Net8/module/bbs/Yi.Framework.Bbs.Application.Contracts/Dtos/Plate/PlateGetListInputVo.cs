using Volo.Abp.Application.Dtos;
using Yi.Framework.Ddd.Application.Contracts;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Plate
{
    public class PlateGetListInputVo : PagedAllResultRequestDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
