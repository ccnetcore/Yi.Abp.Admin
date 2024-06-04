using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;
using Yi.Framework.Bbs.Domain.Shared.Model;

namespace Yi.Framework.Bbs.Domain.SignalRHubs
{
    [HubRoute("/hub/bbs-notice")]
    [Authorize]
    public class BbsNoticeHub : AbpHub
    {
        /// <summary>
        /// hub连接与用户id的映射关系存储
        /// </summary>
        public static ConcurrentDictionary<string, HubUserModel> HubUserModels = new ConcurrentDictionary<string, HubUserModel>();

        /// <summary>
        /// 连接添加用户信息
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            var hubUser = new HubUserModel(Context.ConnectionId, CurrentUser.Id!.Value);
            HubUserModels.TryAdd(CurrentUser.Id.Value.ToString(), hubUser);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 断开连接，去除用户连接信息
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            HubUserModels.TryRemove(CurrentUser.Id.Value.ToString(), out _);
            return Task.CompletedTask;
        }
    }
}