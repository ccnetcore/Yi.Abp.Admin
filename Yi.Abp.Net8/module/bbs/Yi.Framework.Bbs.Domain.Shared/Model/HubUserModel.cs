using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Shared.Model
{
    /// <summary>
    /// 消息通知存储用户信息
    /// </summary>
    public class HubUserModel
    {
        public HubUserModel(string connnectionId,Guid userId)
        {
            ConnnectionId = connnectionId;
            UserId = userId;
        }

        /// <summary>
        /// 客户端连接Id
        /// </summary>
        public string? ConnnectionId { get; }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid? UserId { get; set; }
    }
}
