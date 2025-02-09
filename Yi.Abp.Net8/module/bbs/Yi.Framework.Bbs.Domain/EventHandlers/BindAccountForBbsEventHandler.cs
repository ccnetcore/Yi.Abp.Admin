using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers;

/// <summary>
/// 临时账号绑定到正式账号,钱钱 （累加），禁用临时账号（修改）
/// </summary>
public class BindAccountForBbsEventHandler : ILocalEventHandler<BindAccountEto>, ITransientDependency
{
    private readonly ISqlSugarRepository<BbsUserExtraInfoEntity> _bbsUserRepository;
    private readonly ISqlSugarRepository<UserAggregateRoot> _userRepository;

    public BindAccountForBbsEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> bbsUserRepository,
        ISqlSugarRepository<UserAggregateRoot> userRepository)
    {
        _bbsUserRepository = bbsUserRepository;
        _userRepository = userRepository;
    }

    public async Task HandleEventAsync(BindAccountEto eventData)
    {
        //禁用临时用户
        var oldUser = await _userRepository.GetFirstAsync(x => x.Id == eventData.OldUserId);
        if (oldUser is null || oldUser.State == false)
        {
            throw new UserFriendlyException("无法将无效用户进行绑定");
        }

        oldUser.State = false;
        await _userRepository.UpdateAsync(oldUser);


        //账户钱转移
        var bbsOldUser = await _bbsUserRepository.GetFirstAsync(x => x.UserId == eventData.OldUserId);
        var bbsNewUser = await _bbsUserRepository.GetFirstAsync(x => x.UserId == eventData.NewUserId);
        if (bbsNewUser is not null)
        {
            bbsNewUser.Money += bbsOldUser?.Money ?? 0;
            await _bbsUserRepository.UpdateAsync(bbsNewUser);
        }
    }
}