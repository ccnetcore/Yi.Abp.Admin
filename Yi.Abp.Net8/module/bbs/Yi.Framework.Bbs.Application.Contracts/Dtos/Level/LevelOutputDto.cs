using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Level
{
    public class LevelOutputDto : EntityDto<Guid>
    {
        public Guid Id { get;  set; }

        /// <summary>
        /// 当前等级
        /// </summary>
        public int CurrentLevel { get; set; }

        /// <summary>
        /// 最小所需经验值
        /// </summary>
        public decimal MinExperience { get; set; }

        /// <summary>
        /// 等级名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 等级称号
        /// </summary>
        public string? Nick { get; set; }

        /// <summary>
        /// 等候logo
        /// </summary>
        public string? Logo { get; set; }
    }
}
