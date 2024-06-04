using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.Dtos.Bank;
using Yi.Framework.Bbs.Domain.Entities.Bank;
using Yi.Framework.Bbs.Domain.Managers;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.Bbs.Application.Services.Bank
{
    public class BankService : ApplicationService
    {
        private BankManager _bankManager;
        private BbsUserManager _bbsUserManager;
        private ISqlSugarRepository<BankCardAggregateRoot, Guid> _repository;
        private ISqlSugarRepository<InterestRecordsAggregateRoot, Guid> _interestRepository;
        public BankService(BankManager bankManager, BbsUserManager userManager, ISqlSugarRepository<BankCardAggregateRoot, Guid> repository, ISqlSugarRepository<InterestRecordsAggregateRoot, Guid> interestRepository)
        {
            _bankManager = bankManager;
            _bbsUserManager = userManager;
            _repository = repository;
            _interestRepository = interestRepository;
        }

        /// <summary>
        /// 获取最近24小时汇率记录
        /// </summary>
        /// <returns></returns>
        [HttpGet("bank/interest")]
        public async Task<List<InterestRecordsDto>> GetInterestRecordsAsync()
        {
            var entities = await _interestRepository._DbQueryable.OrderByDescending(x => x.CreationTime).ToPageListAsync(1, 24);
            var output = entities.Adapt<List<InterestRecordsDto>>().OrderBy(x=>x.CreationTime).ToList();
            return output;
        }

        /// <summary>
        /// 获取登录用户全部银行卡信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("bank")]
        public async Task<List<BankCardDto>> GetBankCardListAsync()
        {
            var entities = await _repository._DbQueryable.Where(x => x.UserId == CurrentUser.Id).OrderBy(x=>x.CreationTime).ToListAsync();
            var output = entities.Adapt<List<BankCardDto>>();
            return output;
        }

        /// <summary>
        /// 给用户申请银行卡
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("bank/applying")]
        public async Task ApplyingBankCardAsync()
        {
            var userInfo = await _bbsUserManager.GetBbsUserInfoAsync(CurrentUser.Id!.Value);
            var banCardNum = await _repository.CountAsync(x => x.UserId == CurrentUser.Id!.Value);

            var diffNum = userInfo.Level - banCardNum;
            if (diffNum <= 0)
            {
                throw new UserFriendlyException($"申请失败，当前等级-【{userInfo.Level}】，最多可申领-【{userInfo.Level}】张银行卡，目前已拥有-【{banCardNum}】，请提升你的等级信誉，行长会考虑的");
            }
            else
            {
                await _bankManager.ApplyingBankCardAsync(CurrentUser.Id.Value, diffNum);
            }
        }

        /// <summary>
        /// 给银行卡提款
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("bank/draw/{cardId}")]
        public Task DrawMoneyAsync(Guid cardId)
        {
            return _bankManager.DrawMoneyAsync(cardId);
        }
        /// <summary>
        /// 给银行卡存款
        /// </summary>
        /// <param name="cardId"></param>
        /// <param name="moneyNum"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("bank/deposit/{cardId}/{moneyNum}")]
        public async Task DepositAsync(Guid cardId, decimal moneyNum)
        {
            if (moneyNum < 50)
            {
                throw new UserFriendlyException("存款金额不能小于50");
            }
            var userInfo = await _bbsUserManager.GetBbsUserInfoAsync(CurrentUser.Id!.Value);
            if (userInfo.Money < moneyNum)
            {
                throw new UserFriendlyException("存钱失败！你的钱钱不足，再存进去，就负数啦~");
            }

            await _bankManager.DepositAsync(cardId, moneyNum);
        }


    }
}
