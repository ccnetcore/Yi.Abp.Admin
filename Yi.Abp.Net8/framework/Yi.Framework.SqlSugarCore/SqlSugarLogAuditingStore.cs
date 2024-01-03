using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Yi.Framework.Core.Helper;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore
{
    public class SqlSugarLogAuditingStore : IAuditingStore, ISingletonDependency
    {
    private readonly ILogger<SqlSugarLogAuditingStore> _logger;
        public SqlSugarLogAuditingStore(ILogger<SqlSugarLogAuditingStore> logger, ISqlSugarDbContext sqlSugarDbContext)
        {
            _logger= logger;
        }
        public Task SaveAsync(AuditLogInfo auditInfo)
        {
            _logger.LogDebug("Yi-请求追踪:"+JsonHelper.ObjToStr(auditInfo, "yyyy-MM-dd HH:mm:ss"));
            return Task.CompletedTask;
        }
    }
}
