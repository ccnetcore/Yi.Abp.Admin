using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Bank
{
    public class BankCardDto:EntityDto<Guid>
    {
        /// <summary>
        /// 满期限时间，可空
        /// </summary>
        public DateTime? FulltermTime { get; set; }
        public DateTime? LastDepositTime { get; set; }
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 当前存储的钱
        /// </summary>
        public decimal StorageMoney { get; set; } 


        /// <summary>
        /// 最大可存储的钱钱
        /// </summary>
        public decimal MaxStorageMoney { get; set; }



        /// <summary>
        /// 银行卡状态
        /// </summary>
        public BankCardStateEnum BankCardState { get; set; } = BankCardStateEnum.Unused;

    }
}
