using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Framework.Rbac.Domain.Shared.Dtos;

namespace Yi.Framework.Rbac.Domain.Shared.Etos
{
    public class UserRoleMenuQueryEventArgs
    {
        public UserRoleMenuQueryEventArgs() { }

        public UserRoleMenuQueryEventArgs(params Guid[] userIds)
        {
            UserIds.AddRange(userIds.ToList());
        }
        public List<Guid> UserIds { get; set; } = new List<Guid>();

        public List<UserRoleMenuDto>? Result { get; set; }
    }
}
