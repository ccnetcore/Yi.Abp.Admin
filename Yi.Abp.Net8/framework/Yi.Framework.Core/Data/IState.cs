using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Data
{
    /// <summary>
    /// 状态接口
    /// </summary>
    /// <remarks>
    /// 实现此接口的实体类将支持启用/禁用状态管理
    /// 用于控制数据记录的可用状态
    /// </remarks>
    public interface IState
    {
        /// <summary>
        /// 状态标识
        /// </summary>
        /// <remarks>
        /// true表示启用,false表示禁用
        /// </remarks>
        bool State { get; set; }
    }
}
