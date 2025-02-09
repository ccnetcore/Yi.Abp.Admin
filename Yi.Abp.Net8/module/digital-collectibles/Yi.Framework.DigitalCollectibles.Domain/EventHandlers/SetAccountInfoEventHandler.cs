using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.DigitalCollectibles.Domain.Managers;

namespace Yi.Framework.DigitalCollectibles.Domain.EventHandlers;

public class SetAccountInfoEventHandler : ILocalEventHandler<SetAccountInfoEto>, ITransientDependency
{
    private readonly CollectiblesManager _collectiblesManager;
    private readonly InvitationCodeManager _invitationCodeManager;

    public SetAccountInfoEventHandler(CollectiblesManager collectiblesManager,
        InvitationCodeManager invitationCodeManager)
    {
        _collectiblesManager = collectiblesManager;
        _invitationCodeManager = invitationCodeManager;
    }

    public async Task HandleEventAsync(SetAccountInfoEto eventData)
    {
        var userId = eventData.UserId;
        //设置价值
        eventData.Value = await _collectiblesManager.GetAccountValueAsync(userId);

        //设置积分
        eventData.Points = (await _invitationCodeManager.TryGetOrAddAsync(userId)).PointsNumber;
    }
}