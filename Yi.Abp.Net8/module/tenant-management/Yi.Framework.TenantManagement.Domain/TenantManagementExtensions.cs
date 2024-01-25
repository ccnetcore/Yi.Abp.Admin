using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.TenantManagement.Domain
{
    public static class TenantManagementExtensions
    {
        public static IDisposable ChangeMaster(this ICurrentTenant currentTenant)
        {
            return currentTenant.Change(null, DbConnOptions.MasterTenantDbDefaultName);
        }
    }
}
