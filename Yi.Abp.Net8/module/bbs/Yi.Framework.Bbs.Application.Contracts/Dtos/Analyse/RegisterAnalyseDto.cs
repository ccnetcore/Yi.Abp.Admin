namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Analyse;

public class RegisterAnalyseDto
{
    public RegisterAnalyseDto(DateTime time, int number)
    {
        Time = time;
        Number = number;
    }

    /// <summary>
    /// 时间
    /// </summary>
    public DateTime Time { get; set; }

    /// <summary>
    /// 人数
    /// </summary>
    public int Number { get; set; }
}