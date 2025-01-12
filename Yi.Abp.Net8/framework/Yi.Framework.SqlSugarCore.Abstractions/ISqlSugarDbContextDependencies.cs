using System.Reflection;
using SqlSugar;

namespace Yi.Framework.SqlSugarCore.Abstractions;

public interface ISqlSugarDbContextDependencies
{
    /// <summary>
    /// 执行顺序
    /// </summary>
    int ExecutionOrder { get; }
    
    void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient);
    void DataExecuted(object oldValue, DataAfterModel entityInfo);
    void DataExecuting(object oldValue, DataFilterModel entityInfo);

    void OnLogExecuting(string sql, SugarParameter[] pars);
    void OnLogExecuted(string sql, SugarParameter[] pars);
    
    void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo);
}