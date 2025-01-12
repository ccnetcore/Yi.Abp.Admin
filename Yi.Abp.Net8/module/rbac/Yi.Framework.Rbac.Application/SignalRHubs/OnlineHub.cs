using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Volo.Abp.AspNetCore.SignalR;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.Rbac.Domain.Shared.Model;

namespace Yi.Framework.Rbac.Application.SignalRHubs
{
    [HubRoute("/hub/main")]
    //开放不需要授权
    //[Authorize]
    public class OnlineHub : AbpHub
    {
        public static ConcurrentDictionary<string, OnlineUserModel> ClientUsersDic { get; set; } = new();

        private readonly HttpContext? _httpContext;
        private ILogger<OnlineHub> _logger => LoggerFactory.CreateLogger<OnlineHub>();

        public OnlineHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor?.HttpContext;
        }


        /// <summary>
        /// 成功连接
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            if (_httpContext is null)
            {
                return Task.CompletedTask;
            }
            var name = CurrentUser.UserName;
            var loginUser = new LoginLogAggregateRoot().GetInfoByHttpContext(_httpContext);

            OnlineUserModel user = new(Context.ConnectionId)
            {
                Browser = loginUser?.Browser,
                LoginLocation = loginUser?.LoginLocation,
                Ipaddr = loginUser?.LoginIp,
                LoginTime = DateTime.Now,
                Os = loginUser?.Os,
                UserName = name ?? "Null",
                UserId = CurrentUser.Id ?? Guid.Empty
            };

            //已登录
            if (CurrentUser.IsAuthenticated)
            {
                ClientUsersDic.RemoveAll(u => u.Value.UserId == CurrentUser.Id);
                _logger.LogDebug(
                    $"{DateTime.Now}：{name},{Context.ConnectionId}连接服务端success，当前已连接{ClientUsersDic.Count}个");
            }

            ClientUsersDic.AddOrUpdate(Context.ConnectionId, user, (_, _) => user);

            //当有人加入，向全部客户端发送当前总数
            Clients.All.SendAsync("onlineNum", ClientUsersDic.Count);

            return base.OnConnectedAsync();
        }


        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //已登录
            if (CurrentUser.IsAuthenticated)
            {
                ClientUsersDic.RemoveAll(u => u.Value.UserId == CurrentUser.Id);
                _logger.LogDebug($"用户{CurrentUser?.UserName}离开了，当前已连接{ClientUsersDic.Count}个");
            }
            ClientUsersDic.Remove(Context.ConnectionId, out _);
            Clients.All.SendAsync("onlineNum", ClientUsersDic.Count);
            return base.OnDisconnectedAsync(exception);
        }
    }
}