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
    [Authorize]
    public class OnlineHub : AbpHub
    {
        public static readonly List<OnlineUserModel> clientUsers = new();
        private readonly static object objLock = new object();

        private HttpContext? _httpContext;
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
            lock (objLock)
            {

                var name = CurrentUser.UserName;
                var loginUser = new LoginLogEntity().GetInfoByHttpContext(_httpContext);
                var user = clientUsers.Any(u => u is not null && u.ConnnectionId == Context.ConnectionId);
                //判断用户是否存在，否则添加集合
                if (!user)
                {
                    OnlineUserModel users = new(Context.ConnectionId)
                    {
                        Browser = loginUser?.Browser,
                        LoginLocation = loginUser?.LoginLocation,
                        Ipaddr = loginUser?.LoginIp,
                        LoginTime = DateTime.Now,
                        Os = loginUser?.Os,
                        UserName = name ?? "Null",
                        UserId = CurrentUser.Id ?? Guid.Empty
                    };


                    //先移除之前的用户id，一个用户只能一个
                    clientUsers.RemoveAll(u => u.UserId == CurrentUser.Id);
                    clientUsers.Add(users);
                    _logger.LogInformation($"{DateTime.Now}：{name},{Context.ConnectionId}连接服务端success，当前已连接{clientUsers.Count}个");

                    //当有人加入，向全部客户端发送当前总数
                    Clients.All.SendAsync("onlineNum", clientUsers.Count);
                }
            }
            return base.OnConnectedAsync();
        }


        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            lock (objLock)
            {
                var user = clientUsers.Where(p => p.ConnnectionId == Context.ConnectionId).FirstOrDefault();
                //判断用户是否存在，否则添加集合
                if (user != null)
                {
                    clientUsers.RemoveAll(u => u.UserId == CurrentUser.Id || u.ConnnectionId == u.ConnnectionId);
                    Clients.All.SendAsync("onlineNum", clientUsers.Count);
                    _logger.LogInformation($"用户{user?.UserName}离开了，当前已连接{clientUsers.Count}个");

                }
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendAllTest(string test)
        {
            await Clients.All.SendAsync("ReceiveAllInfo", test);
        }

    }
}