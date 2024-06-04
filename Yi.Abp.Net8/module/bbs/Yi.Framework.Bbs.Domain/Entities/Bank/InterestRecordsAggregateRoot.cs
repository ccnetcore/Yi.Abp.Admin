using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace Yi.Framework.Bbs.Domain.Entities.Bank
{
    /// <summary>
    /// 利息记录
    /// </summary>
    [SugarTable("InterestRecords")]
    public class InterestRecordsAggregateRoot : AggregateRoot<Guid>, IHasCreationTime
    {
        public InterestRecordsAggregateRoot()
        { }
        public InterestRecordsAggregateRoot(decimal comparisonValue, decimal inputValue, bool isFluctuate = false)
        {
            ComparisonValue = comparisonValue;
            Value = inputValue;
            IsFluctuate = isFluctuate;
        }
        [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
        public override Guid Id { get; protected set; }
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 第三方的比较值
        /// </summary>
        public decimal ComparisonValue { get; set; }

        /// <summary>
        /// 当前汇率值
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 是否波动期
        /// </summary>
        public bool IsFluctuate { get; set; }


    }
}
