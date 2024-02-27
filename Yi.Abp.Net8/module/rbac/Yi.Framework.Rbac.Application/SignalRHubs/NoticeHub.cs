using Volo.Abp.AspNetCore.SignalR;

namespace Yi.Framework.Rbac.Application.SignalRHubs
{
    [HubRoute("/hub/notice")]
    public class NoticeHub : AbpHub
    {
        /// <summary>
        /// 由于发布功能，主要是服务端项客户端主动推送
        /// </summary>
        public NoticeHub()
        {
        }
    }
}
