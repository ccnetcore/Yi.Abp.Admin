using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Analyse;

public class PointsTopUserDto:BaseAnalyseTopUserDto
{
    public int Points { get; set; }
}