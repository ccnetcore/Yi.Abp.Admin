using System.Data.Common;
using Volo.Abp;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore;

/// <summary>
/// SqlSugar数据库上下文创建上下文
/// </summary>
public class SqlSugarDbContextCreationContext
{
    private static readonly AsyncLocal<SqlSugarDbContextCreationContext> CurrentContextHolder = 
        new AsyncLocal<SqlSugarDbContextCreationContext>();

    /// <summary>
    /// 获取当前上下文
    /// </summary>
    public static SqlSugarDbContextCreationContext Current => CurrentContextHolder.Value!;

    /// <summary>
    /// 连接字符串名称
    /// </summary>
    public string ConnectionStringName { get; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    /// 现有数据库连接
    /// </summary>
    public DbConnection? ExistingConnection { get; internal set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public SqlSugarDbContextCreationContext(
        string connectionStringName,
        string connectionString)
    {
        ConnectionStringName = connectionStringName;
        ConnectionString = connectionString;
    }

    /// <summary>
    /// 使用指定的上下文
    /// </summary>
    public static IDisposable Use(SqlSugarDbContextCreationContext context)
    {
        var previousContext = Current;
        CurrentContextHolder.Value = context;
        return new DisposeAction(() => CurrentContextHolder.Value = previousContext);
    }
}
