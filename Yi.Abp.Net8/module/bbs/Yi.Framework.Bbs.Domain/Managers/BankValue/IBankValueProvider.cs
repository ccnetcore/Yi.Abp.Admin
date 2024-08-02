using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Managers.BankValue
{
    public interface IBankValueProvider
    {
        /// <summary>
        /// 标准值
        /// </summary>
        decimal StandardValue { get; }

        /// <summary>
        /// 获取第三方值
        /// </summary>
        /// <returns></returns>
        Task<decimal> GetValueAsync();

    }
}
