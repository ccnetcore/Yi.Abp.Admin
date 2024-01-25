using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Shared.Etos
{
    public class ExperienceChangeEventArgs
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 变化经验，可负
        /// </summary>
        public decimal Number { get; set; }
    }
}
