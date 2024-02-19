using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Entities.Forum;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    /// <summary>
    /// 评论创建的领域事件
    /// </summary>
    public class CommentCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<CommentEntity>>,
          ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userRepository;
        public CommentCreatedEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task HandleEventAsync(EntityCreatedEventData<CommentEntity> eventData)
        {
            var commentEntity = eventData.Entity;

            //给创建者发布数量+1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.CommentNumber == it.CommentNumber + 1)
                                        .Where(it => it.UserId == commentEntity.CreatorId)
                                        .ExecuteCommandAsync();
        }
    }
}
