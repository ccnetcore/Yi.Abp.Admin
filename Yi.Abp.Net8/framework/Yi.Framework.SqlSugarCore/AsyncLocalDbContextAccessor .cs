using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore
{
    /// <summary>
    /// 异步本地数据库上下文访问器
    /// 用于在异步流中保存和访问数据库上下文
    /// </summary>
    public sealed class AsyncLocalDbContextAccessor
    {
        private readonly AsyncLocal<ISqlSugarDbContext?> _currentScope;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static AsyncLocalDbContextAccessor Instance { get; } = new();

        /// <summary>
        /// 获取或设置当前数据库上下文
        /// </summary>
        public ISqlSugarDbContext? Current
        {
            get => _currentScope.Value;
            set => _currentScope.Value = value;
        }

        /// <summary>
        /// 初始化异步本地数据库上下文访问器
        /// </summary>
        private AsyncLocalDbContextAccessor()
        {
            _currentScope = new AsyncLocal<ISqlSugarDbContext?>();
        }
    }
} 