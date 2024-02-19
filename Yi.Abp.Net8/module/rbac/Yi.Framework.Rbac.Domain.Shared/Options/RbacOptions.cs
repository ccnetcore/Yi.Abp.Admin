using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Options
{
    public class RbacOptions
    {
        /// <summary>
        /// 超级管理员默认密码
        /// </summary>
        public string AdminPassword { get; set; } = "123456";

        /// <summary>
        /// 租户超级管理员默认密码
        /// </summary>
        public string TenantAdminPassword { get; set; } = "123456";

        /// <summary>
        /// 是否开启登录验证码
        /// </summary>
        public bool EnableCaptcha { get; set; } = false;

        /// <summary>
        /// 是否开启用户注册功能
        /// </summary>
        public bool EnableRegister { get; set; } = false;

        /// <summary>
        /// 是否开启数据库备份
        /// </summary>
        public bool EnableDataBaseBackup { get; set; } = false;
    }
}
