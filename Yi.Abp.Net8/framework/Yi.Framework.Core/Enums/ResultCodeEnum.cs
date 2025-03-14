using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    /// <summary>
    /// API返回状态码枚举
    /// </summary>
    /// <remarks>
    /// 定义API接口统一的返回状态码
    /// 遵循HTTP状态码规范
    /// </remarks>
    public enum ResultCodeEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        Success = 200,

        /// <summary>
        /// 未授权访问
        /// </summary>
        NoPermission = 401,

        /// <summary>
        /// 访问被拒绝
        /// </summary>
        Denied = 403,

        /// <summary>
        /// 操作失败
        /// </summary>
        NotSuccess = 500
    }
}
