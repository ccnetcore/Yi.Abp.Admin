using Volo.Abp.Uow;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Uow
{
    /// <summary>
    /// SqlSugar事务API实现
    /// </summary>
    public class SqlSugarTransactionApi : ITransactionApi, ISupportsRollback
    {
        private readonly ISqlSugarDbContext _dbContext;

        public SqlSugarTransactionApi(ISqlSugarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        public ISqlSugarDbContext GetDbContext()
        {
            return _dbContext;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SqlSugarClient.Ado.CommitTranAsync();
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SqlSugarClient.Ado.RollbackTranAsync();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _dbContext.SqlSugarClient.Ado.Dispose();
        }
    }
}
