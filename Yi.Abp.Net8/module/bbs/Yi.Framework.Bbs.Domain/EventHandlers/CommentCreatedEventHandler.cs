using TencentCloud.Tbm.V20180129.Models;
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
    /// 评论创建的领域事件
    /// </summary>
    public class CommentCreatedEventHandler : ILocalEventHandler<EntityCreatedEventData<CommentAggregateRoot>>,
          ITransientDependency
    {
        private ILocalEventBus _localEventBus;
        private ISqlSugarRepository<DiscussAggregateRoot> _discussRepository;
        private ISqlSugarRepository<UserAggregateRoot> _userRepository;
        public CommentCreatedEventHandler(ILocalEventBus localEventBus, ISqlSugarRepository<DiscussAggregateRoot> discussRepository, ISqlSugarRepository<UserAggregateRoot> userRepository)
        {
            _userRepository = userRepository;
            _localEventBus = localEventBus;
            _discussRepository = discussRepository;
        }
        public async Task HandleEventAsync(EntityCreatedEventData<CommentAggregateRoot> eventData)
        {
            var commentEntity = eventData.Entity;

            //给创建者发布数量+1
            await _userRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                                        .SetColumns(it => it.CommentNumber == it.CommentNumber + 1)
                                        .Where(it => it.UserId == commentEntity.CreatorId)
                                        .ExecuteCommandAsync();
            var disucssDto = await _discussRepository._DbQueryable
                 .Where(x => x.Id == commentEntity.DiscussId)
                .LeftJoin<UserAggregateRoot>((dicuss, user) => dicuss.CreatorId == user.Id)
               .Select((dicuss, user) =>
               new
               {
                   DiscussUserId = user.Id,
                   DiscussTitle = dicuss.Title,

               })
               .FirstAsync();

            var commentUser = await _userRepository.GetFirstAsync(x => x.Id == commentEntity.CreatorId);

            //截取30个长度
            var content = commentEntity.Content.Length >= 30 ? commentEntity.Content.Substring(0, 30)+"..." : commentEntity.Content;
            //通知主题作者，有人评论
            await _localEventBus.PublishAsync(new BbsNoticeEventArgs(disucssDto.DiscussUserId, string.Format(DiscussConst.CommentNotice, disucssDto.DiscussTitle, commentUser.UserName, content,commentEntity.DiscussId)), false);

            //如果为空，表示根路径，没有回复者
            if (commentEntity.ParentId != Guid.Empty)
            {
                //通知回复者，有人评论
                await _localEventBus.PublishAsync(new BbsNoticeEventArgs(commentEntity.ParentId, string.Format(DiscussConst.CommentNoticeToReply, disucssDto.DiscussTitle, commentUser.UserName, content,commentEntity.DiscussId)), false);

            }
         
        }
    }
}
