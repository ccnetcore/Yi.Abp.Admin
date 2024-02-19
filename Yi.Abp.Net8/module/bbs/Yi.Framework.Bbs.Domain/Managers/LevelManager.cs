using Mapster;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Caches;
using Yi.Framework.Bbs.Domain.Shared.Consts;
using Yi.Framework.Bbs.Domain.Shared.Etos;

namespace Yi.Framework.Bbs.Domain.Managers
{
    public class LevelManager : DomainService
    {
        private BbsUserManager _bbsUserManager;
        private ILocalEventBus _localEventBus;
        private List<LevelCacheItem> _levelCacheItem;
        public LevelManager(BbsUserManager bbsUserManager, ILocalEventBus localEventBus, IDistributedCache<List<LevelCacheItem>> levelCache)
        {
            _bbsUserManager = bbsUserManager;
            _localEventBus = localEventBus;
            _levelCacheItem = levelCache.Get(LevelConst.LevelCacheKey);
        }


        /// <summary>
        /// 使用钱钱投喂等级
        /// </summary>
        /// <returns></returns>
        public async Task ChangeLevelByMoneyAsync(Guid userId, int moneyNumber)
        {
            //通过用户id获取用户信息的经验和等级
            var userInfo = await _bbsUserManager.GetBbsUserInfoAsync(userId);

            //钱钱和经验的比例为1：1
            //根据钱钱修改经验
            var currentNewExperience = userInfo.Experience + moneyNumber * 1;

            //修改钱钱，如果钱钱不足，直接会丢出去
            await _localEventBus.PublishAsync(new MoneyChangeEventArgs { UserId = userId, Number = -moneyNumber },false);

            //更改最终的经验再变化等级
            var levelList = _levelCacheItem.OrderByDescending(x => x.CurrentLevel).ToList();
            var currentNewLevel = 1;
            foreach (var level in levelList)
            {
                if (currentNewExperience >= level.MinExperience)
                {
                    currentNewLevel = level.CurrentLevel;
                    break;
                }
            }

            var exUserInfo = await _bbsUserManager._bbsUserInfoRepository.GetAsync(x => x.UserId == userInfo.Id);
            exUserInfo.Level = currentNewLevel;
            exUserInfo.Experience = currentNewExperience;
            await _bbsUserManager._bbsUserInfoRepository.UpdateAsync(exUserInfo);

        }
    }
}
