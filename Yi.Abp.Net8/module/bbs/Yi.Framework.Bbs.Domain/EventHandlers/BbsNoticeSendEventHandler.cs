using Microsoft.AspNetCore.SignalR;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Bbs.Domain.SignalRHubs;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    /// <summary>
    /// bbs消息推送处理
    /// </summary>
    public class BbsNoticeSendEventHandler : ILocalEventHandler<BbsNoticeEventArgs>,
      ITransientDependency
    {
        private IHubContext<BbsNoticeHub> _hubContext;
        private ISqlSugarRepository<BbsNoticeAggregateRoot, Guid> _repository;
        public BbsNoticeSendEventHandler(IHubContext<BbsNoticeHub> hubContext, ISqlSugarRepository<BbsNoticeAggregateRoot, Guid> sugarRepository)
        {
            _hubContext = hubContext;
            _repository = sugarRepository;
        }
        public async Task HandleEventAsync(BbsNoticeEventArgs eventData)
        {
          

            //是否需要离线存储
           bool isStore = true;
           var now = DateTime.Now;
           switch (eventData.NoticeType)
            {
                case Shared.Enums.NoticeTypeEnum.Personal:
                    if (BbsNoticeHub.HubUserModels.TryGetValue(eventData.AcceptUserId.ToString(), out var hubUserModel))
                    {
                        _hubContext.Clients.Client(hubUserModel.ConnnectionId).SendAsync(NoticeTypeEnum.Personal.ToString(), eventData.Message,now);
                    }
                    break;
                case Shared.Enums.NoticeTypeEnum.Broadcast:
                    _hubContext.Clients.All.SendAsync(NoticeTypeEnum.Broadcast.ToString(), eventData.Message);
                    break;
                
                case Shared.Enums.NoticeTypeEnum.Money:
                    if (BbsNoticeHub.HubUserModels.TryGetValue(eventData.AcceptUserId.ToString(), out var hubUserModel2))
                    {
                        _hubContext.Clients.Client(hubUserModel2.ConnnectionId).SendAsync(NoticeTypeEnum.Money.ToString(), eventData.Message,now);
                    }

                    isStore = false;
                    break;
                default:
                    break;
            }

            if (isStore)
            {  //离线存储
                var entity= await _repository.InsertReturnEntityAsync(new BbsNoticeAggregateRoot(eventData.NoticeType, eventData.Message, eventData.AcceptUserId){CreationTime = now});
            }

        }
    }
}
