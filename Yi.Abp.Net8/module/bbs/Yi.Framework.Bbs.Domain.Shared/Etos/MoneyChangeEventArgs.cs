using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Yi.Framework.Bbs.Domain.Shared.Etos
{
    public class MoneyChangeEventArgs
    {
        public MoneyChangeEventArgs() { }
        public MoneyChangeEventArgs(Guid userId, decimal changeNumber) { UserId = userId; 

            Number = Math.Round(changeNumber, 2); }

        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 变化金额，可负
        /// </summary>
        public decimal Number { get; set; }
    }
}
