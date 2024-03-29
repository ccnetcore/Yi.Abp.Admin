﻿using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Yi.Framework.AuditLogging.Domain.Shared;
using Yi.Framework.Bbs.Domain.Shared;
using Yi.Framework.Rbac.Domain.Shared;

namespace Yi.Abp.Domain.Shared
{
    [DependsOn(
        typeof(YiFrameworkRbacDomainSharedModule),
        typeof(YiFrameworkBbsDomainSharedModule),
        typeof(YiFrameworkAuditLoggingDomainSharedModule),

        typeof(AbpDddDomainSharedModule))]
    public class YiAbpDomainSharedModule : AbpModule
    {

    }
}