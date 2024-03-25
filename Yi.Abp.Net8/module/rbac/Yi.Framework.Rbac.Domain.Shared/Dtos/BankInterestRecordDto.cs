using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Dtos
{
    /// <summary>
    /// 当前银行记录
    /// </summary>
    public class BankInterestRecordDto
    {
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
        /// 判断时间是否过期
        /// </summary>
        /// <returns></returns>
        public bool IsExpire()
        {
            return (DateTime.Now-CreationTime).TotalHours >= 1;
        }

    }
}
