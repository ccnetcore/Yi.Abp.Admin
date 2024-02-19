using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Yi.Framework.TenantManagement.Application.Contracts.Dtos
{
    public class TenantSelectOutputDto : EntityDto<Guid>
    {
        public string Name { get; set; }
    }
}
