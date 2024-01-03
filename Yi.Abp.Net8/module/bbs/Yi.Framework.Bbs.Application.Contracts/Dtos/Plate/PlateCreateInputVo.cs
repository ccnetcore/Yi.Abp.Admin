namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Plate
{
    /// <summary>
    /// Plate输入创建对象
    /// </summary>
    public class PlateCreateInputVo
    {
        public string Name { get; set; }
        public string? Logo { get; set; }
        public string? Introduction { get; set; }

        public string Code { get; set; }

        public int OrderNum { get; set; }

        public bool IsDisableCreateDiscuss { get; set; }
    }
}
