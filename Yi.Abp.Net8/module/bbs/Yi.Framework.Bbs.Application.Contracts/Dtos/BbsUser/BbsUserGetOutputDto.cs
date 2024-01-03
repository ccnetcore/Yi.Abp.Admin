using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Bbs.Domain.Shared.Enums;
using Yi.Framework.Rbac.Application.Contracts.Dtos.User;

namespace Yi.Framework.Bbs.Application.Contracts.Dtos.BbsUser
{
    public class BbsUserGetOutputDto: UserGetOutputDto
    {
        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 用户限制
        /// </summary>
        public UserLimitEnum UserLimit { get; set; }
    }
}
