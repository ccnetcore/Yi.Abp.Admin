using System.Reflection;
using SqlSugar;
using Volo.Abp.DependencyInjection;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

public abstract class SqlSugarDbContext : ISqlSugarDbContextDependencies
{
    protected IAbpLazyServiceProvider LazyServiceProvider { get; }

    public SqlSugarDbContext(IAbpLazyServiceProvider lazyServiceProvider)
    {
        this.LazyServiceProvider = lazyServiceProvider;
    }


    protected ISqlSugarClient SqlSugarClient { get;private set; }
    public int ExecutionOrder => 0;

    public void OnSqlSugarClientConfig(ISqlSugarClient sqlSugarClient)
    {
        SqlSugarClient = sqlSugarClient;
        CustomDataFilter(sqlSugarClient);
    }
    protected virtual void CustomDataFilter(ISqlSugarClient sqlSugarClient)
    {
    }
    
    public virtual void DataExecuted(object oldValue, DataAfterModel entityInfo)
    {
    }

    public virtual void DataExecuting(object oldValue, DataFilterModel entityInfo)
    {
    }

    public virtual void OnLogExecuting(string sql, SugarParameter[] pars)
    {
    }

    public virtual void OnLogExecuted(string sql, SugarParameter[] pars)
    {
    }

    public virtual void EntityService(PropertyInfo propertyInfo, EntityColumnInfo entityColumnInfo)
    {
    }
}