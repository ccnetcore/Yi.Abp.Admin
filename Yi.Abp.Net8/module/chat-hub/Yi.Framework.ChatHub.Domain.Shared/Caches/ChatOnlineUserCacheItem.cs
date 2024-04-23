using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.ChatHub.Domain.Shared.Caches
{
    public class ChatOnlineUserCacheItem
    {
        public ChatOnlineUserCacheItem()
        { 
        
        
        }
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserIcon { get; set; }

        /// <summary>
        /// 客户端id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 连接时间
        /// </summary>
        public DateTime CreationTime { get; }=DateTime.Now;

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }

    public class ChatOnlineUserCacheKey
    {
        public string CacheKeyPrefix;
        public ChatOnlineUserCacheKey(string cacheKeyPrefix )
        {
            CacheKeyPrefix=cacheKeyPrefix;
        }

        public string GetKey()
        {
            return $"{CacheKeyPrefix}ChatOnline";
        }

        public string GetField(Guid userId)
        {
            return $"{userId}";
        }
    }
}
