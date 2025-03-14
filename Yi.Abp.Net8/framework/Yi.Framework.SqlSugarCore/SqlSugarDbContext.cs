using System.Reflection;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

/// <summary>
/// SqlSugar数据库上下文基类
/// </summary>
public abstract class SqlSugarDbContext : ISqlSugarDbContextDependencies
{
    /// <summary>
    /// 服务提供者
    /// </summary>
    protected IAbpLazyServiceProvider LazyServiceProvider { get; }

    /// <summary>
    /// 数据库客户端实例
    /// </summary>
    protected ISqlSugarClient SqlSugarClient { get; private set; }

    /// <summary>
    /// 执行顺序
    /// </summary>
    public virtual int ExecutionOrder => 0;

    protected SqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider)
    {
        LazyServiceProvider = lazyServiceProvider;
    }

    /// <summary>
    /// 配置SqlSugar客户端
    /// </summary>
    public virtual void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
    {
        SqlSugarClient = sqlSugarClient;
        CustomDataFilter(sqlSugarClient);
    }

    /// <summary>
    /// 自定义数据过滤器
    /// </summary>
    protected virtual void CustomDataFilter(ISqlSugarClient sqlSugarClient)
    {
    }

    /// <summary>
    /// 数据执行后事件
    /// </summary>
    public virtual void DataExecuted(object oldValue, DataAfterModel entityInfo)
    {
    }

    /// <summary>
    /// 数据执行前事件
    /// </summary>
    public virtual void DataExecuting(object oldValue, DataFilterModel entityInfo)
    {
    }

    /// <summary>
    /// SQL执行前事件
    /// </summary>
    public virtual void OnLogExecuting(string sql, SugarParameter[] pars)
    {
    }

    /// <summary>
    /// SQL执行后事件
    /// </summary>
    public virtual void OnLogExecuted(string sql, SugarParameter[] pars)
    {
    }

    /// <summary>
    /// 实体服务配置
    /// </summary>
    public virtual void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo)
    {
    }
}