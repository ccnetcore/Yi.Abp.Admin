﻿using Volo.Abp.Modularity;
using Yi.Abp.Domain.Shared;
using Yi.Framework.Bbs.Application.Contracts;
using Yi.Framework.Ddd.Application.Contracts;
using Yi.Framework.Rbac.Application.Contracts;
using Yi.Framework.TenantManagement.Application.Contracts;

namespace Yi.Abp.Application.Contracts
{
    [DependsOn(
        typeof(YiAbpDomainSharedModule),

        typeof(YiFrameworkRbacApplicationContractsModule),
        typeof(YiFrameworkBbsApplicationContractsModule),

        typeof(YiFrameworkTenantManagementApplicationContractsModule),
        typeof(YiFrameworkDddApplicationContractsModule))]
    public class YiAbpApplicationContractsModule:AbpModule
    {

    }
}