using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.EventHandlers
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
            var userIfno = await _userInfoRepository.GetFirstAsync(x => x.UserId == eventData.UserId);
            
            //如果变化后的钱钱少于0，直接丢出去
            if ((userIfno.Money + eventData.Number)<0)
            {
                throw new UserFriendlyException(MoneyConst.Money_Low_Zero);
            }
            //原子性sql
            await _userInfoRepository._Db.Updateable<BbsUserExtraInfoEntity>()
                  .SetColumns(it => it.Money == it.Money + eventData.Number)
                  .Where(x => x.UserId == eventData.UserId).ExecuteCommandAsync();
        }
    }
}
