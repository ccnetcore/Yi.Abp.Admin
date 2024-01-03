namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Banner
{
    /// <summary>
    /// Banner输入创建对象
    /// </summary>
    public class BannerCreateInputVo
    {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Color { get; set; }
    }
}
