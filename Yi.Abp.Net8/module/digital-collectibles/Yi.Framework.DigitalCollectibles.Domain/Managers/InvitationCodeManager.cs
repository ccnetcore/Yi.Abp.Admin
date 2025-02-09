using Volo.Abp.Domain.Services;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.Managers;

/// <summary>
/// 邀请码领域服务
/// </summary>
public class InvitationCodeManager : DomainService
{
    public readonly ISqlSugarRepository<InvitationCodeAggregateRoot> _repository;

    public InvitationCodeManager(ISqlSugarRepository<InvitationCodeAggregateRoot> repository)
    {
        _repository = repository;
    }


    /// <summary>
    /// 填写邀请码
    /// </summary>
    public async Task SetAsync(Guid writeUserId, string invitationCode)
    {
        //统一大写
        invitationCode=  invitationCode.ToUpper();
        var entityOrNull = await _repository.GetFirstAsync(x => x.InvitationCode == invitationCode);
        if (entityOrNull is null)
        {
            throw new UserFriendlyException("无效邀请码，请检查");
        }
        if (entityOrNull.UserId==writeUserId)
        {
            throw new UserFriendlyException("你不能邀请自己");
        }

        //被邀请的人
        var entity = entityOrNull;
        entity.SetInvited();

        //填写邀请码的人
        var writeEntity = await TryGetOrAddAsync(writeUserId);
        writeEntity.SetInvite();

        await _repository.UpdateRangeAsync(new List<InvitationCodeAggregateRoot> { entity, writeEntity });
    }

    public async Task<InvitationCodeAggregateRoot> TryGetOrAddAsync(Guid userId, int InitPointsNumber = 0)
    {
        var entity = await _repository.FindAsync(x => x.UserId == userId);
        if (entity is null)
        {
            string invitationCode = string.Empty;

            //循环到邀请码没有重复为止
            var isExist = true;
            while (isExist)
            {
                invitationCode = CreateInvitationCode(4);
                if (!await _repository.IsAnyAsync(x => x.InvitationCode == invitationCode))
                {
                    isExist = false;
                }
            }

            var insertEntity = new InvitationCodeAggregateRoot(userId, invitationCode)
            {
                PointsNumber = InitPointsNumber
            };
            entity = await _repository.InsertReturnEntityAsync(insertEntity);
        }

        return entity;
    }

    private string CreateInvitationCode(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Random random = new Random();
        char[] stringChars = new char[length];

        for (int i = 0; i < length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}