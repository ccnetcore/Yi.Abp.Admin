using System.Security.AccessControl;
using FreeRedis;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Yi.Framework.ChatHub.Domain.Entities;
using Yi.Framework.ChatHub.Domain.Shared.Caches;
using Yi.Framework.ChatHub.Domain.Shared.Consts;
using Yi.Framework.ChatHub.Domain.Shared.Enums;
using Yi.Framework.ChatHub.Domain.Shared.Model;
using Yi.Framework.ChatHub.Domain.SignalRHubs;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.ChatHub.Domain.Managers
{
    public class UserMessageManager : DomainService
    {
        private IHubContext<ChatCenterHub> _hubContext;
        public ISqlSugarRepository<MessageAggregateRoot> _repository;
        public UserMessageManager(IHubContext<ChatCenterHub> hubContext, ISqlSugarRepository<MessageAggregateRoot> repository)
        {
            _hubContext = hubContext;
            _repository = repository;
        }
        /// <summary>
        /// 使用懒加载防止报错
        /// </summary>
        private IRedisClient RedisClient => LazyServiceProvider.LazyGetRequiredService<IRedisClient>();
        private string CacheKeyPrefix => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDistributedCacheOptions>>().Value.KeyPrefix;

        public async Task SendMessageAsync(MessageContext context)
        {
            switch (context.MessageType)
            {
                case MessageTypeEnum.Personal:
                    var userModel = await GetUserAsync(context.ReceiveId.Value);
                    if (userModel is not null)
                    {
                        await _hubContext.Clients.Client(userModel.ClientId).SendAsync(ChatConst.ClientActionReceiveMsg, context.MessageType, context);
                    }
                    break;
                case MessageTypeEnum.Group:
                    throw new NotImplementedException();
                    break;
                case MessageTypeEnum.All:
                    await _hubContext.Clients.All.SendAsync(ChatConst.ClientActionReceiveMsg, context.MessageType, context);
                    break;
                case MessageTypeEnum.Ai:
                    var userModel2 = await GetUserAsync(context.ReceiveId.Value);
                    if (userModel2 is not null)
                    {
                        await _hubContext.Clients.Client(userModel2.ClientId).SendAsync(ChatConst.ClientActionReceiveMsg, context.MessageType, context);
                    }
                    break;

                default:
                    break;
            }

        }


        public async Task CreateMessageStoreAsync(MessageContext context)
        {
            await _repository.InsertAsync(context.Adapt<MessageAggregateRoot>());
        }

        public async Task<List<ChatOnlineUserCacheItem>> GetAllUserAsync()
        {
            var key = new ChatOnlineUserCacheKey(CacheKeyPrefix);
            var cacheUsers = (await RedisClient.HGetAllAsync(key.GetKey())).Select(x => System.Text.Json.JsonSerializer.Deserialize<ChatOnlineUserCacheItem>(x.Value)).ToList();
            return cacheUsers;
        }

        public async Task<ChatOnlineUserCacheItem?> GetUserAsync(Guid userId)
        {
            var key = new ChatOnlineUserCacheKey(CacheKeyPrefix);
            var cacheUser = System.Text.Json.JsonSerializer.Deserialize<ChatOnlineUserCacheItem>(await RedisClient.HGetAsync(key.GetKey(), key.GetField(userId)));
            return cacheUser;
        }
    }


}
