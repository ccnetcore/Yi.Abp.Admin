using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Yi.Framework.Bbs.Domain.Entities.Bank;
using Yi.Framework.Bbs.Domain.Managers.BankValue;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Bbs.Domain.Shared.Etos;
using Yi.Framework.Rbac.Domain.Shared.Dtos;
using Yi.Framework.SqlSugarCore.Abstractions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yi.Framework.Bbs.Domain.Managers
{
    /// <summary>
    /// 银行领域，进阶了哦~
    /// </summary>
    public class BankManager : DomainService
    {
        private ISqlSugarRepository<BankCardAggregateRoot> _repository;
        private ILocalEventBus _localEventBus;
        private ISqlSugarRepository<InterestRecordsAggregateRoot> _interestRepository;
        private IBankValueProvider _bankValueProvider;
        public BankManager(ISqlSugarRepository<BankCardAggregateRoot> repository, ILocalEventBus localEventBus, ISqlSugarRepository<InterestRecordsAggregateRoot> interestRepository, IBankValueProvider bankValueProvider)
        {
            _repository = repository;
            _localEventBus = localEventBus;
            _interestRepository = interestRepository;
            _bankValueProvider=bankValueProvider;
        }

        /// <summary>
        /// 获取当前银行汇率
        /// </summary>
        public BankInterestRecordDto CurrentRate => GetCurrentInterestRate();

        /// <summary>
        /// 用于存储当前汇率数据
        /// </summary>
        private BankInterestRecordDto? _currentRateStore;

        /// <summary>
        /// 获取当前的银行汇率，如果为空会从数据库拿最新一条
        /// </summary>
        /// <returns></returns>
        private BankInterestRecordDto GetCurrentInterestRate()
        {
            var output = new BankInterestRecordDto();
            //先判断时间是否与当前时间差1小时，小于1小时直接返回即可,可以由一个单例类提供
            if (this._currentRateStore is null || this._currentRateStore.IsExpire())
            {
                var currentInterestRecords = CreateInterestRecordsAsync().Result;
                output.ComparisonValue = currentInterestRecords.ComparisonValue;
                output.CreationTime = currentInterestRecords.CreationTime;
                output.Value = currentInterestRecords.Value;

                _currentRateStore=new BankInterestRecordDto() { ComparisonValue= currentInterestRecords .ComparisonValue,
                CreationTime=currentInterestRecords.CreationTime,Value=currentInterestRecords.Value};

            }
            return output;
        }

        /// <summary>
        /// 获取第三方的值
        /// </summary>
        /// <returns></returns>
        private decimal GetThirdPartyValue()
        {
            return _bankValueProvider.GetValueAsync().Result;
        }

        /// <summary>
        /// 强制创建一个记录，不管时间到没到
        /// </summary>
        /// <returns></returns>
        public async Task<InterestRecordsAggregateRoot> CreateInterestRecordsAsync()
        {
            //获取最新的实体
            var lastEntity = await _interestRepository._DbQueryable.OrderByDescending(x => x.CreationTime).FirstAsync();
            decimal oldValue = 1.3m;

            //获取第三方的值
            var thirdPartyValue = GetThirdPartyValue();

            //获取上一次第三方标准值
            var lastThirdPartyStandardValue = thirdPartyValue;


            //获取实际值的变化率
            decimal changeRate = 0;
            //说明不是第一次
            if (lastEntity is not null)
            {
                oldValue = lastEntity.Value;
                changeRate = (thirdPartyValue - lastEntity.ComparisonValue) / (thirdPartyValue);
            }

            //判断市场是否波动
            bool isFluctuate = IsMarketVolatility();
            //市场波动
            if (isFluctuate)
            {
                changeRate = 2 * changeRate;
            }


            //根据上一次的老值进行变化率比较
            var currentValue = oldValue + (oldValue* changeRate);

            var entity = new InterestRecordsAggregateRoot(lastThirdPartyStandardValue, currentValue);
            var output = await _interestRepository.InsertReturnEntityAsync(entity);

            return output;
        }

        /// <summary>
        /// 判断是否为波动市场,市场波动，变化率翻倍
        /// </summary>
        /// <returns></returns>
        private static bool IsMarketVolatility()
        {
            double probability = 0.1;
            Random random = new Random();
            return random.NextDouble() < probability;
        }

        /// <summary>
        /// 给用户申请银行卡
        /// </summary>
        /// <returns></returns>
        public async Task ApplyingBankCardAsync(Guid userId, int cardNumber)
        {
            var entities = Enumerable.Range(1, cardNumber).Select(x => new BankCardAggregateRoot(userId)).ToList();
            await _repository.InsertManyAsync(entities);
        }

        /// <summary>
        /// 进行银行卡提款
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task DrawMoneyAsync(Guid cardId)
        {
            var entity = await _repository.GetByIdAsync(cardId);
            if (entity.BankCardState == BankCardStateEnum.Unused)
            {
                throw new UserFriendlyException("当前银行卡状态不能提款");
            }

            //这里其实不存在这个状态，只有等待状态，不需要去主动触发，前端判断即可
            if (entity.BankCardState == BankCardStateEnum.Full)
            {
                throw new UserFriendlyException("当前银行卡状态不能存款");
            }

            //可以提款
            if (entity.BankCardState == BankCardStateEnum.Wait)
            {
                decimal changeMoney = 0;
                //判断是否存满时间
                if (entity.IsStorageFull())
                {
                    changeMoney = this.CurrentRate.Value * entity.StorageMoney;
                }
                else
                {
                    changeMoney = entity.StorageMoney;
                }

                //提款
                entity.SetDrawMoney();
                await _repository.UpdateAsync(entity);

                //打钱，该卡状态钱更新，并提款加到用户钱钱里
                await _localEventBus.PublishAsync(new MoneyChangeEventArgs(entity.UserId, changeMoney));



            }
        }

        /// <summary>
        /// 给银行卡存款
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="moneyNum"></param>
        /// <returns></returns>
        public async Task DepositAsync(Guid cardId, decimal moneyNum)
        {
            var entity = await _repository.GetByIdAsync(cardId);
            if (entity.BankCardState != BankCardStateEnum.Unused)
            {
                throw new UserFriendlyException("当前银行卡状态不能存款");
            }
            //存款
            entity.SetStorageMoney(moneyNum);

            await _repository.UpdateAsync(entity);
            await _localEventBus.PublishAsync(new MoneyChangeEventArgs(entity.UserId, -moneyNum));

        }

    }
}
