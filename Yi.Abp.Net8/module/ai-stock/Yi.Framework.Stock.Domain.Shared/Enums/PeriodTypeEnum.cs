namespace Yi.Framework.Stock.Domain.Shared
{
    /// <summary>
    /// 时间周期类型枚举
    /// </summary>
    /// <remarks>
    /// 用于定义股票价格记录的时间周期类型
    /// </remarks>
    public enum PeriodTypeEnum
    {
        /// <summary>
        /// 分钟
        /// </summary>
        Minute = 0,
        
        /// <summary>
        /// 小时
        /// </summary>
        Hour = 1,
        
        /// <summary>
        /// 天
        /// </summary>
        Day = 2,
        
        /// <summary>
        /// 周
        /// </summary>
        Week = 3,
        
        /// <summary>
        /// 月
        /// </summary>
        Month = 4,
        
        /// <summary>
        /// 年
        /// </summary>
        Year = 5
    }
} 