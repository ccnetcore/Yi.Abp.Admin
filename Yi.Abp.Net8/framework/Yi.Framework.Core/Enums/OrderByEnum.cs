using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    /// <summary>
    /// 排序方向枚举
    /// </summary>
    /// <remarks>
    /// 用于定义数据查询时的排序方向
    /// 常用于列表数据排序
    /// </remarks>
    public enum OrderByEnum
    {
        /// <summary>
        /// 升序排列
        /// </summary>
        Asc = 0,

        /// <summary>
        /// 降序排列
        /// </summary>
        Desc = 1
    }
}
