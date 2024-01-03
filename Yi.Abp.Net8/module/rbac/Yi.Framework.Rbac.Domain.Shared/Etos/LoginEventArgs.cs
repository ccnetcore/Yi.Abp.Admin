using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Yi.Framework.Rbac.Domain.Shared.Etos
{
    public class LoginEventArgs
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }

        public DateTime CreationTime { get; set; }
        /// <summary>
        /// 登录地点 
        ///</summary>
        public string? LoginLocation { get; set; }
        /// <summary>
        /// 登录Ip 
        ///</summary>
        public string? LoginIp { get; set; }
        /// <summary>
        /// 浏览器 
        ///</summary>
        public string? Browser { get; set; }
        /// <summary>
        /// 操作系统 
        ///</summary>

        public string? Os { get; set; }
        /// <summary>
        /// 登录信息 
        ///</summary>
        public string? LogMsg { get; set; }
    }
}
