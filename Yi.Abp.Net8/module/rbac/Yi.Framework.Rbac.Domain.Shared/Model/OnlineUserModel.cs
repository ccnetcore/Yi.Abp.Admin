using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Model
{
    public class OnlineUserModel
    {
        public OnlineUserModel()
        {

        }
        public OnlineUserModel(string connnectionId)
        {
            ConnnectionId = connnectionId;
        }

        /// <summary>
        /// 客户端连接Id
        /// </summary>
        public string? ConnnectionId { get; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime LoginTime { get; set; }
        public string? Ipaddr { get; set; }
        public string? LoginLocation { get; set; }

        public string? Os { get; set; }
        public string? Browser { get; set; }


    }
}
