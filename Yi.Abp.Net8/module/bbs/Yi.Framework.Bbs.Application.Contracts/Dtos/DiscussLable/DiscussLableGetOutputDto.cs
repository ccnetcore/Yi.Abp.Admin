using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.DiscussLable;

public class DiscussLableGetOutputDto:EntityDto<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Color { get; set; }
    public string? BackgroundColor { get; set; }
}