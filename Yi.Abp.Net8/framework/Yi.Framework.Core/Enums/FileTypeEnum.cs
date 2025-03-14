using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Core.Enums
{
    /// <summary>
    /// 文件类型枚举
    /// </summary>
    /// <remarks>
    /// 用于定义系统支持的文件类型分类
    /// 主要用于文件上传和存储时的类型区分
    /// </remarks>
    public enum FileTypeEnum
    {
        /// <summary>
        /// 普通文件
        /// </summary>
        file = 0,

        /// <summary>
        /// 图片文件
        /// </summary>
        image = 1,

        /// <summary>
        /// 缩略图文件
        /// </summary>
        thumbnail = 2,

        /// <summary>
        /// Excel文件
        /// </summary>
        excel = 3,

        /// <summary>
        /// 临时文件
        /// </summary>
        temp = 4
    }
}
