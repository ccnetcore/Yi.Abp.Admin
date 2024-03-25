using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.Bank
{
    public class InterestRecordsDto : EntityDto<Guid>
    {
        public DateTime CreationTime { get; set; }

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
