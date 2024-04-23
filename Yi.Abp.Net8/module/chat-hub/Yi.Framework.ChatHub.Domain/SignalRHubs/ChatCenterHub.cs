using FreeRedis;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Yi.Framework.ChatHub.Domain.Shared.Caches;
using Yi.Framework.ChatHub.Domain.Shared.Consts;
using Yi.Framework.ChatHub.Domain.Shared.Model;

namespace Yi.Framework.ChatHub.Domain.SignalRHubs
{
    [HubRoute("/hub/chat")]
    [Authorize]
    public class ChatCenterHub : AbpHub
    {
        /// <summary>
        /// 使用懒加载防止报错
        /// </summary>
        private IRedisClient RedisClient => LazyServiceProvider.LazyGetRequiredService<IRedisClient>();
        /// <summary>
        /// 缓存前缀
        /// </summary>
        private string CacheKeyPrefix => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDistributedCacheOptions>>().Value.KeyPrefix;
        public ChatCenterHub()
        {
        }

        /// <summary>
        /// 用户进入聊天室
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            var key = new ChatOnlineUserCacheKey(CacheKeyPrefix);
            var item = new ChatOnlineUserCacheItem()
            {
                UserId = CurrentUser.Id!.Value,
                ClientId = Context.ConnectionId,
                UserName = CurrentUser.UserName!
            };


            await RedisClient.HSetAsync(key.GetKey(), key.GetField(CurrentUser.Id!.Value), item);


            //连接时，还需要去查询用户包含在的群组，将群主全部加入.Todo
            await Groups.AddToGroupAsync(Context.ConnectionId, ChatConst.AllGroupName);
            await Clients.All.SendAsync("liveUser", item.Adapt<ChatUserModel>());


        }

        /// <summary>
        /// 用户退出聊天室
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var key = new ChatOnlineUserCacheKey(CacheKeyPrefix);
            await RedisClient.HDelAsync(key.GetKey(), key.GetField(CurrentUser.Id!.Value));
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ChatConst.AllGroupName);
            await Clients.All.SendAsync("offlineUser", CurrentUser.Id!.Value);
        }
    }
}
