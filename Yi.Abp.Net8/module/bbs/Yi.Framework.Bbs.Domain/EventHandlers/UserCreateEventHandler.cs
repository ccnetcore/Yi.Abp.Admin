using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Rbac.Domain.Shared.Etos;

namespace Yi.Framework.Bbs.Domain.EventHandlers
{
    public class UserCreateEventHandler : ILocalEventHandler<UserCreateEventArgs>, ITransientDependency
    {
        private IRepository<BbsUserExtraInfoEntity> _repository;
        public UserCreateEventHandler(IRepository<BbsUserExtraInfoEntity> repository)
        {
            _repository = repository;
        }
        public async Task HandleEventAsync(UserCreateEventArgs eventData)
        {
            //创建主表
            var bbsUser = new BbsUserExtraInfoEntity(eventData.UserId)
            {

            };
            await _repository.InsertAsync(bbsUser);
        }
    }
}
