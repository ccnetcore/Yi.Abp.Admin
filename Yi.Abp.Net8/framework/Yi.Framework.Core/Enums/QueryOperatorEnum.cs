using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    public enum QueryOperatorEnum
    {
        /// <summary>
        /// 相等
        /// </summary>
        Equal,
        /// <summary>
        /// 匹配
        /// </summary>
        Like,
        /// <summary>
        /// 大于
        /// </summary>
        GreaterThan,
        /// <summary>
        /// 大于或等于
        /// </summary>
        GreaterThanOrEqual,
        /// <summary>
        /// 小于
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于或等于
        /// </summary>
        LessThanOrEqual,
        /// <summary>
        /// 等于集合
        /// </summary>
        In,
        /// <summary>
        /// 不等于集合
        /// </summary>
        NotIn,
        /// <summary>
        /// 左边匹配
        /// </summary>
        LikeLeft,
        /// <summary>
        /// 右边匹配
        /// </summary>
        LikeRight,
        /// <summary>
        /// 不相等
        /// </summary>
        NoEqual,
        /// <summary>
        /// 为空或空
        /// </summary>
        IsNullOrEmpty,
        /// <summary>
        /// 不为空
        /// </summary>
        IsNot,
        /// <summary>
        /// 不匹配
        /// </summary>
        NoLike,
        /// <summary>
        /// 时间段 值用 "|" 隔开
        /// </summary>
        DateRange
    }
}
