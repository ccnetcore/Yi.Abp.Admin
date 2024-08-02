using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using Yi.Framework.Bbs.Domain.Entities.Integral;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Domain.Managers
{
    public class IntegralManager : DomainService
    {
        public ISqlSugarRepository<LevelAggregateRoot> _levelRepository;
        public ISqlSugarRepository<SignInAggregateRoot> _signInRepository;
        private readonly ILocalEventBus _localEventBus;
        public IntegralManager(ISqlSugarRepository<LevelAggregateRoot> levelRepository, ISqlSugarRepository<SignInAggregateRoot> signInRepository, ILocalEventBus localEventBus)
        {
            _levelRepository = levelRepository;
            _localEventBus = localEventBus;
            _signInRepository = signInRepository;
        }


        /// <summary>
        /// 签到
        /// </summary>
        /// <returns></returns>
        [UnitOfWork]
        public async Task<decimal> SignInAsync(Guid userId)
        {
            //签到，添加用户钱钱
            //发送一个充值的领域事件即可


            //签到添加的钱钱，跟连续签到有关系
            //每天随机（3-10），连续签到每次累加多1点，最多一天30

            //额外
            //如果随机数数字以9结尾，额外再获取乘1倍

            //这种逻辑，就是属于核心领域业务了


            var sigInLast = await _signInRepository._DbQueryable.Where(x => x.CreatorId == userId).OrderByDescending(x => x.CreationTime).FirstAsync();

            //verify 校验是否允许签到了
            if (sigInLast is not null)
            {
                VerifySignInTime(sigInLast.CreationTime);
            }

            //连续签到次数
            var continuousNumber = GetContinuousNumber(sigInLast);
            //签到奖励值
            var value = GetSignInValue(continuousNumber);


            //插入记录
            var entity = new SignInAggregateRoot() { ContinuousNumber = continuousNumber };
            await _signInRepository.InsertAsync(entity);

            //发布一个其他领域的事件
            await _localEventBus.PublishAsync(new MoneyChangeEventArgs() { UserId = userId, Number = value },false);
            return value;
        }

        /// <summary>
        /// 校验签到时间
        /// </summary>
        /// <param name="dataTime"></param>
        /// <exception cref="UserFriendlyException"></exception>
        private void VerifySignInTime(DateTime dataTime)
        {
            if (dataTime.Date == DateTime.Now.Date)
            {
                throw new UserFriendlyException("今日你已经签到过了~");
            }

        }

        /// <summary>
        /// 获取签到值
        /// </summary>
        /// <param name="continuousNumber"></param>
        /// <returns></returns>
        private decimal GetSignInValue(int continuousNumber)
        {
            //基础数值
            var baseValue = new Random().Next(300, 1100) / 100m;


            //累加额外的奖励
            var extraValue = 0m;
            if (baseValue.ToString().EndsWith("9"))
            {
                extraValue = 1 * baseValue;
            }

            //累加连续签到的奖励
            var signInValue = continuousNumber * 1m;


            //获取添加的值
            var value = baseValue + extraValue + signInValue;
            if (value >= 30)
            {
                value = 30;
            }
            return value;
        }

        /// <summary>
        /// 获取连续次数
        /// </summary>
        private int GetContinuousNumber(SignInAggregateRoot sigInLast)
        {
            var continuousNumber = 1;

            //已签到过
            if (sigInLast is not null)
            {
                //签到过，且昨天已签到过，直接使用昨天的连续次数+1
                if (sigInLast.CreationTime.Day == DateTime.Now.AddDays(-1).Day)
                {
                    continuousNumber = sigInLast.ContinuousNumber + 1;
                }
            }
            return continuousNumber;
        }
    }
}
