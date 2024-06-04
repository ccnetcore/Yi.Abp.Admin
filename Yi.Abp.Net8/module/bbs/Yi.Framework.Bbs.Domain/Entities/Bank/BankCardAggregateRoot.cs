using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Yi.Framework.Bbs.Domain.Shared.Enums;

namespace Yi.Framework.Bbs.Domain.Entities.Bank
{
    /// <summary>
    /// 银行卡
    /// </summary>
    [SugarTable("BankCard")]
    public class BankCardAggregateRoot : AggregateRoot<Guid>, IHasCreationTime
    {
        public BankCardAggregateRoot()
        {
        }

        public BankCardAggregateRoot(Guid userId)
        {
            this.UserId = userId;
        }
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 上一次存款日期
        /// </summary>
        public DateTime? LastDepositTime { get; set; }

        /// <summary>
        /// 上一次取款日期
        /// </summary>
        public DateTime? LastDrawTime { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 当前存储的钱
        /// </summary>
        public decimal StorageMoney { get; set; } = 0;


        /// <summary>
        /// 最大可存储的钱钱
        /// </summary>
        public decimal MaxStorageMoney { get; set; } = 100;


        /// <summary>
        /// 满期限时间，可空
        /// </summary>
        public DateTime? FulltermTime { get; set; }




        /// <summary>
        /// 银行卡状态
        /// </summary>
        public BankCardStateEnum BankCardState { get; set; } = BankCardStateEnum.Unused;

        public bool IsStorageFull()
        {
            if (FulltermTime is null)
            {
                return false;
            }
            return DateTime.Now >= FulltermTime;
        }
        public void SetDrawMoney()
        {
            this.BankCardState = BankCardStateEnum.Unused;

            LastDrawTime = DateTime.Now;
            this.FulltermTime = null;
            this.StorageMoney = 0;
        }
        public void SetStorageMoney(decimal storageMoney)
        {
            if (storageMoney > MaxStorageMoney)
            {
                throw new UserFriendlyException($"存款数不能大于该卡的上限-【{MaxStorageMoney}】钱钱");
            }

            StorageMoney = storageMoney;

            LastDepositTime = DateTime.Now;
            FulltermTime = LastDepositTime + TimeSpan.FromDays(3);
            this.BankCardState = BankCardStateEnum.Wait;
        }
    }
}
