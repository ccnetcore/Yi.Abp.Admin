using Volo.Abp.Uow;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Uow
{
    public class SqlSugarTransactionApi : ITransactionApi, ISupportsRollback
    {
        private ISqlSugarDbContext _sqlsugarDbContext;

        public SqlSugarTransactionApi(ISqlSugarDbContext sqlsugarDbContext)
        {
            _sqlsugarDbContext = sqlsugarDbContext;
        }

        public ISqlSugarDbContext GetDbContext()
        {

            return _sqlsugarDbContext;
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _sqlsugarDbContext.SqlSugarClient.Ado.CommitTranAsync();
        }

        public void Dispose()
        {
            _sqlsugarDbContext.SqlSugarClient.Ado.Dispose();
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            await _sqlsugarDbContext.SqlSugarClient.Ado.RollbackTranAsync();
        }
    }
}
