using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Users;
using Yi.Framework.Rbac.Domain.Shared.Consts;

namespace Yi.Framework.Bbs.Domain.Extensions
{
    public static class CurrestUserExtensions
    {
        /// <summary>
        /// 获取用户权限codes
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static List<string> GetPermissions(this ICurrentUser currentUser)
        {
            return currentUser.FindClaims(TokenTypeConst.Permission).Select(x => x.Value).ToList();

        }
    }
}
