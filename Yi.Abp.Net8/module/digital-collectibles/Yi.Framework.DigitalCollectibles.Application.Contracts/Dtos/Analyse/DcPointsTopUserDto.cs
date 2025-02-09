namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;

public class DcPointsTopUserDto
{
    public Guid UserId { get; set; }
    public int Points { get; set; }

    public int Order { get; set; }
}