using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    /// <summary>
    /// 被点赞
    /// </summary>
    public class AgreeCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<AgreeEntity>>,
      ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userRepository;
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        public AgreeCreatedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userRepository, ISqlSugarRepository<AgreeEntity> agreeRepository)
        {
            _userRepository = userRepository;
            _agreeRepository = agreeRepository;
        }
        public async Task HandleEventAsync(EntityCreatedEventData<AgreeEntity> eventData)
        {
            var agreeEntity = eventData.Entity;
            var userId = await _agreeRepository._DbQueryable.LeftJoin<DiscussEntity>((agree, discuss) => agree.DiscussId == discuss.Id)
                   .Select((agree, discuss) => discuss.CreatorId).FirstAsync();

            //给创建者发布数量+1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.DiscussNumber == it.DiscussNumber + 1)
                                        .Where(it => it.UserId == userId)
                                        .ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 取消点赞
    /// </summary>
    public class AgreeDeletedEventHandler : ILocalEventHandler<EntityCreatedEventData<AgreeEntity>>,
  ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userRepository;
        private ISqlSugarRepository<AgreeEntity> _agreeRepository;
        public AgreeDeletedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userRepository, ISqlSugarRepository<AgreeEntity> agreeRepository)
        {
            _userRepository = userRepository;
            _agreeRepository = agreeRepository;
        }
        public async Task HandleEventAsync(EntityCreatedEventData<AgreeEntity> eventData)
        {
            var agreeEntity = eventData.Entity;
            var userId = await _agreeRepository._DbQueryable.LeftJoin<DiscussEntity>((agree, discuss) => agree.DiscussId == discuss.Id)
                   .Select((agree, discuss) => discuss.CreatorId).FirstAsync();

            //给创建者发布数量-1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.DiscussNumber == it.DiscussNumber - 1)
                                        .Where(it => it.UserId == userId)
                                        .ExecuteCommandAsync();
        }
    }
}
