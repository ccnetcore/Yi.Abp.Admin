using SqlSugar;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Integral
{
    /// <summary>
    /// 等级表
    /// </summary>
    [SugarTable("Level")]
    public class LevelAggregateRoot : AggregateRoot<Guid>
    {
        public LevelAggregateRoot() { }

        public LevelAggregateRoot(int currentLevel, string name, decimal minExperience)
        {
            this.CurrentLevel = currentLevel;
            this.Name = name;
            this.MinExperience = minExperience;
        }

        [SugarColumn(IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }


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
