using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    public enum ResultCodeEnum
    {
        /// <summary>
        /// 操作成功。
        /// </summary>
        Success = 200,

        /// <summary>
        /// 操作不成功
        /// </summary>
        NotSuccess = 500,

        /// <summary>
        /// 无权限
        /// </summary>
        NoPermission = 401,

        /// <summary>
        /// 被拒绝
        /// </summary>
        Denied = 403
    }
}
