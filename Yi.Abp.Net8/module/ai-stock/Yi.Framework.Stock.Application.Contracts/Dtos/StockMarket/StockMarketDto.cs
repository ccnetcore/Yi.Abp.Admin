using System;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Stock.Application.Contracts.Dtos.StockMarket
{
    /// <summary>
    /// 股市信息DTO
    /// </summary>
    public class StockMarketDto : EntityDto<Guid>
    {
        /// <summary>
        /// 股市代码
        /// </summary>
        public string MarketCode { get; set; }

        /// <summary>
        /// 股市名称
        /// </summary>
        public string MarketName { get; set; }

        /// <summary>
        /// 股市描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
} 