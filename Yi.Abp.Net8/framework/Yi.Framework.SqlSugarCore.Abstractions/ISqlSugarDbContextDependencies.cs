using System.Reflection;
using SqlSugar;

namespace Yi.Framework.SqlSugarCore.Abstractions;

/// <summary>
/// SqlSugar数据库上下文依赖接口
/// 定义数据库操作的各个生命周期钩子
/// </summary>
public interface ISqlSugarDbContextDependencies
{
    /// <summary>
    /// 获取执行顺序
    /// </summary>
    int ExecutionOrder { get; }
    
    /// <summary>
    /// SqlSugar客户端配置时触发
    /// </summary>
    /// <param name="sqlSugarClient">SqlSugar客户端实例</param>
    void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient);

    /// <summary>
    /// 数据执行后触发
    /// </summary>
    /// <param name="oldValue">原始值</param>
    /// <param name="entityInfo">实体信息</param>
    void DataExecuted(object oldValue, DataAfterModel entityInfo);

    /// <summary>
    /// 数据执行前触发
    /// </summary>
    /// <param name="oldValue">原始值</param>
    /// <param name="entityInfo">实体信息</param>
    void DataExecuting(object oldValue, DataFilterModel entityInfo);

    /// <summary>
    /// SQL执行前触发
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">SQL参数</param>
    void OnLogExecuting(string sql, SugarParameter[] parameters);

    /// <summary>
    /// SQL执行后触发
    /// </summary>
    /// <param name="sql">SQL语句</param>
    /// <param name="parameters">SQL参数</param>
    void OnLogExecuted(string sql, SugarParameter[] parameters);
    
    /// <summary>
    /// 实体服务配置
    /// </summary>
    /// <param name="propertyInfo">属性信息</param>
    /// <param name="entityColumnInfo">实体列信息</param>
    void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo);
}