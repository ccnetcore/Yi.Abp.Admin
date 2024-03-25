using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Domain.Shared.Enums
{
    public enum BankCardStateEnum
    {
        //闲置
        Unused = 0,

        //等待中
        Wait,

        //存储时间已满
        Full,
    }
}
