using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Uow;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Uow
{
    public class SqlSugarDatabaseApi : IDatabaseApi
    {
        public ISqlSugarDbContext DbContext { get; }

        public SqlSugarDatabaseApi(ISqlSugarDbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}
