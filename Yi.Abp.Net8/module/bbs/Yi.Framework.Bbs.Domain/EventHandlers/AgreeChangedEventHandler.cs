using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Rbac.Domain.Entities;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    /// <summary>
    /// 被点赞
    /// </summary>
    public class AgreeCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<AgreeEntity>>,
      ITransientDependency
    {
        private ISqlSugarRepository<UserAggregateRoot> _userRepository;
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userInfoRepository;
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        private ILocalEventBus _localEventBus;
        public AgreeCreatedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userInfoRepository, ISqlSugarRepository<AgreeEntity> agreeRepository, ILocalEventBus localEventBus, ISqlSugarRepository<UserAggregateRoot> userRepository)
        {
            _userInfoRepository = userInfoRepository;
            _agreeRepository = agreeRepository;
            _localEventBus = localEventBus;
            _userRepository = userRepository;
        }
        public async Task HandleEventAsync(EntityCreatedEventData<AgreeEntity> eventData)
        {
            var agreeEntity = eventData.Entity;

            //查询主题的信息
            var discussAndAgreeDto = await _agreeRepository._DbQueryable
                .LeftJoin<DiscussAggregateRoot>((agree, discuss) => agree.DiscussId == discuss.Id)
                   .Select((agree, discuss) =>
                   new
                   {
                       DiscussId=discuss.Id,
                       DiscussTitle = discuss.Title,
                       DiscussCreatorId = discuss.CreatorId,
                   })
                   .FirstAsync();

            //查询点赞者用户
            var agreeUser = await _userRepository.GetFirstAsync(x => x.Id == agreeEntity.CreatorId);

            //给创建者点赞数量+1
            await _userInfoRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.AgreeNumber == it.AgreeNumber + 1)
                                        .Where(it => it.UserId == discussAndAgreeDto.DiscussCreatorId)
                                        .ExecuteCommandAsync();

            //通知主题作者，有人点赞
            await _localEventBus.PublishAsync(new BbsNoticeEventArgs(discussAndAgreeDto.DiscussCreatorId!.Value, string.Format(DiscussConst.AgreeNotice, discussAndAgreeDto.DiscussTitle, agreeUser.UserName,discussAndAgreeDto.DiscussId)), false);

        }
    }
    /// <summary>
    /// 取消点赞
    /// </summary>
    public class AgreeDeletedEventHandler : ILocalEventHandler<EntityDeletedEventData<AgreeEntity>>,
  ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userRepository;
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        private ILocalEventBus _localEventBus;
        public AgreeDeletedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userRepository, ISqlSugarRepository<AgreeEntity> agreeRepository, ILocalEventBus localEventBus)
        {
            _userRepository = userRepository;
            _agreeRepository = agreeRepository;
            _localEventBus = localEventBus;
        }
        public async Task HandleEventAsync(EntityDeletedEventData<AgreeEntity> eventData)
        {
            var agreeEntity = eventData.Entity;
            var userId = await _agreeRepository._DbQueryable.LeftJoin<DiscussAggregateRoot>((agree, discuss) => agree.DiscussId == discuss.Id)
                   .Select((agree, discuss) => discuss.CreatorId).FirstAsync();

            //给创建者点赞数量-1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.AgreeNumber == it.AgreeNumber - 1)
                                        .Where(it => it.UserId == userId)
                                        .ExecuteCommandAsync();
        }
    }
}
