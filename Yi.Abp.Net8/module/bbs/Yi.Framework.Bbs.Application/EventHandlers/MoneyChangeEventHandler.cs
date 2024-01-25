using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.EventHandlers
{
    public class MoneyChangeEventHandler : ILocalEventHandler<MoneyChangeEventArgs>, ITransientDependency
    {
        private ISqlSugarRepository<BbsUserExtraInfoEntity> _userInfoRepository;
        public MoneyChangeEventHandler(ISqlSugarRepository<BbsUserExtraInfoEntity> userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }
        public async Task HandleEventAsync(MoneyChangeEventArgs eventData)
        {
            //原子性sql
            await _userInfoRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                  .SetColumns(it => it.Money == it.Money + eventData.Number)
                  .Where(x => x.UserId == eventData.UserId).ExecuteCommandAsync();
        }
    }
}
