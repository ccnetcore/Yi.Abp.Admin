using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.Rbac.Domain.Shared.Etos
{
    /// <summary>
    /// 用户创建的id
    /// </summary>
    public class UserCreateEventArgs
    {
        public UserCreateEventArgs(Guid userId)
        {
            UserId = userId;
        }
        public Guid UserId { get; set; }
    }
}
