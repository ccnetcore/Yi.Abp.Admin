using System.Data;

namespace Yi.Framework.TenantManagement.Application.Contracts.Dtos
{
    public class TenantUpdateInput
    {
        public string? Name { get;  set; }
        public int? EntityVersion { get;  set; }

        public string? TenantConnectionString { get;  set; }

        public SqlSugar.DbType? DbType { get;  set; }
    }
}
