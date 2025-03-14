using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    /// <summary>
    /// 查询操作符枚举
    /// </summary>
    /// <remarks>
    /// 定义查询条件中支持的操作符类型
    /// 用于构建动态查询条件
    /// </remarks>
    public enum QueryOperatorEnum
    {
        /// <summary>
        /// 等于
        /// </summary>
        Equal = 0,

        /// <summary>
        /// 模糊匹配
        /// </summary>
        Like = 1,

        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan = 2,

        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual = 3,

        /// <summary>
        /// 小于
        /// </summary>
        LessThan = 4,

        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual = 5,

        /// <summary>
        /// 在指定集合中
        /// </summary>
        In = 6,

        /// <summary>
        /// 不在指定集合中
        /// </summary>
        NotIn = 7,

        /// <summary>
        /// 左侧模糊匹配
        /// </summary>
        LikeLeft = 8,

        /// <summary>
        /// 右侧模糊匹配
        /// </summary>
        LikeRight = 9,

        /// <summary>
        /// 不等于
        /// </summary>
        NoEqual = 10,

        /// <summary>
        /// 为null或空
        /// </summary>
        IsNullOrEmpty = 11,

        /// <summary>
        /// 不为null
        /// </summary>
        IsNot = 12,

        /// <summary>
        /// 不匹配
        /// </summary>
        NoLike = 13,

        /// <summary>
        /// 日期范围
        /// </summary>
        /// <remarks>
        /// 使用"|"分隔起始和结束日期
        /// </remarks>
        DateRange = 14
    }
}
