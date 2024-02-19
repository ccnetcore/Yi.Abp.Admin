using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.TenantManagement.Application.Contracts.Dtos
{
    public class TenantGetListOutputDto:EntityDto<Guid>
    {
        public  string Name { get;  set; }
        public int EntityVersion { get;  set; }

        public string TenantConnectionString { get;  set; }

        public SqlSugar.DbType DbType { get;  set; }
        public DateTime CreationTime { get; set; }
    }
}
