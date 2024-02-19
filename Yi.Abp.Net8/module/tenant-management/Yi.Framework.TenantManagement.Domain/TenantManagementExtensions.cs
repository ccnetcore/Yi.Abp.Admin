using Volo.Abp.MultiTenancy;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.TenantManagement.Domain
{
    public static class TenantManagementExtensions
    {
        public static IDisposable ChangeMaster(this ICurrentTenant currentTenant)
        {
            return currentTenant.Change(null, DbConnOptions.MasterTenantName);
        }

        public static IDisposable ChangeDefalut(this ICurrentTenant currentTenant)
        {
            return currentTenant.Change(null, DbConnOptions.DefaultTenantName);
        }
    }
}
