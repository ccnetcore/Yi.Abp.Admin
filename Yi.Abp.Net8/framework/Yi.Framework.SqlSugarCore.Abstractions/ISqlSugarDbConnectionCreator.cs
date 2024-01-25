using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    public interface ISqlSugarDbConnectionCreator
    {
        DbConnOptions Options { get; }
        Action<ISqlSugarClient> OnSqlSugarClientConfig { get; set; }
        Action<object, DataAfterModel> DataExecuted { get; set; }
        Action<object, DataFilterModel> DataExecuting { get; set; }
        Action<string, SugarParameter[]> OnLogExecuting { get; set; }
        Action<string, SugarParameter[]> OnLogExecuted { get; set; }
        Action<PropertyInfo, EntityColumnInfo> EntityService { get; set; }

        ConnectionConfig Build(Action<ConnectionConfig>? action = null);
        void SetDbAop(ISqlSugarClient currentDb);
    }
}
