using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    /// <summary>
    /// 主题创建的领域事件
    /// </summary>
    public class DiscussCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<DiscussAggregateRoot>>,
        ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userRepository;
        private ILocalEventBus _localEventBus;

        public DiscussCreatedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userRepository,
            ILocalEventBus localEventBus)
        {
            _userRepository = userRepository;
            _localEventBus = localEventBus;
        }

        public async Task HandleEventAsync(EntityCreatedEventData<DiscussAggregateRoot> eventData)
        {
            var disucussEntity = eventData.Entity;

            //给创建者发布数量+1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                .SetColumns(it => it.DiscussNumber == it.DiscussNumber + 1)
                .Where(it => it.UserId == disucussEntity.CreatorId)
                .ExecuteCommandAsync();

            //最后发布任务触发事件
            await _localEventBus.PublishAsync(
                new AssignmentEventArgs(AssignmentRequirementTypeEnum.Discuss, disucussEntity.CreatorId!.Value), false);
        }
    }
}