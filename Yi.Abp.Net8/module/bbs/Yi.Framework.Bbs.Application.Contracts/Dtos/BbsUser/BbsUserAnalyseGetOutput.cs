using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser
{
    public class BbsUserAnalyseGetOutput
    {
        /// <summary>
        /// 注册人数
        /// </summary>
        public long RegisterNumber { get; set; }


        /// <summary>
        /// 在线人数
        /// </summary>
        public long OnlineNumber { get; set; }

        /// <summary>
        /// 昨天新增用户
        /// </summary>
        public long YesterdayNewUser { get; set; }
    }
}
