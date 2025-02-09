namespace Yi.Framework.DigitalCollectibles.Application.Contracts.Dtos.Analyse;

public class DcValueTopUserDto
{
    public Guid UserId { get; set; }
    public decimal Value { get; set; }
    
    public int Order { get; set; }
}