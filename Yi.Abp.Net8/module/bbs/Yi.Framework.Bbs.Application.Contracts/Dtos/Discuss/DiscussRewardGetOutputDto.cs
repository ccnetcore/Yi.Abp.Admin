namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Discuss;

public class DiscussRewardGetOutputDto
{
        /// <summary>
        /// 是否已解决
        /// </summary>
        public bool IsResolved{ get; set; }
        /// <summary>
        /// 悬赏最小价值
        /// </summary>
        public decimal MinValue { get; set; }

        /// <summary>
        /// 悬赏最大价值
        /// </summary>
        public decimal? MaxValue { get; set; }

        /// <summary>
        /// 作者联系方式
        /// </summary>
        public string Contact { get; set; }
}