using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Data
{
    /// <summary>
    /// 排序接口
    /// </summary>
    /// <remarks>
    /// 实现此接口的实体类将支持排序功能
    /// 通常用于列表数据的展示顺序控制
    /// </remarks>
    public interface IOrderNum
    {
        /// <summary>
        /// 排序号
        /// </summary>
        /// <remarks>
        /// 数字越小越靠前,默认为0
        /// </remarks>
        int OrderNum { get; set; }
    }
}
