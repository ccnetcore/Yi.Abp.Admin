using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SqlSugar;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Uow
{
    public class UnitOfWorkSqlsugarDbContextProvider<TDbContext> : ISugarDbContextProvider<TDbContext> where TDbContext : ISqlSugarDbContext
    {
        /// <summary>
        /// 日志记录器
        /// </summary>
        public ILogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>> Logger { get; set; }

        /// <summary>
        /// 服务提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// 数据库上下文访问器实例
        /// </summary>
        private static AsyncLocalDbContextAccessor ContextInstance => AsyncLocalDbContextAccessor.Instance;

        private readonly TenantConfigurationWrapper _tenantConfigurationWrapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IConnectionStringResolver _connectionStringResolver;
        private readonly ICancellationTokenProvider _cancellationTokenProvider;
        private readonly ICurrentTenant _currentTenant;

        public UnitOfWorkSqlsugarDbContextProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            ICancellationTokenProvider cancellationTokenProvider,
            ICurrentTenant currentTenant, 
            TenantConfigurationWrapper tenantConfigurationWrapper)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _connectionStringResolver = connectionStringResolver;
            _cancellationTokenProvider = cancellationTokenProvider;
            _currentTenant = currentTenant;
            _tenantConfigurationWrapper = tenantConfigurationWrapper;
            Logger = NullLogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>>.Instance;
        }

        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        public virtual async Task<TDbContext> GetDbContextAsync()
        {
            // 获取当前租户配置
            var tenantConfiguration = await _tenantConfigurationWrapper.GetAsync();
            
            // 获取连接字符串信息
            var connectionStringName = tenantConfiguration.GetCurrentConnectionName();
            var connectionString = tenantConfiguration.GetCurrentConnectionString();
            var dbContextKey = $"{this.GetType().Name}_{connectionString}";

            var unitOfWork = _unitOfWorkManager.Current;
            if (unitOfWork == null)
            {
                throw new AbpException(
                    "DbContext 只能在工作单元内工作，当前DbContext没有工作单元，如需创建新线程并发操作，请手动创建工作单元");
            }

            // 尝试从当前工作单元获取数据库API
            var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

            // 当前没有数据库API则创建新的
            if (databaseApi == null)
            {
                databaseApi = new SqlSugarDatabaseApi(
                    await CreateDbContextAsync(unitOfWork, connectionStringName, connectionString)
                );
                unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);
            }

            return (TDbContext)((SqlSugarDatabaseApi)databaseApi).DbContext;
        }

        /// <summary>
        /// 创建数据库上下文
        /// </summary>
        protected virtual async Task<TDbContext> CreateDbContextAsync(
            IUnitOfWork unitOfWork, 
            string connectionStringName, 
            string connectionString)
        {
            var creationContext = new SqlSugarDbContextCreationContext(connectionStringName, connectionString);
            using (SqlSugarDbContextCreationContext.Use(creationContext))
            {
                return await CreateDbContextAsync(unitOfWork);
            }
        }

        /// <summary>
        /// 根据工作单元创建数据库上下文
        /// </summary>
        protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Options.IsTransactional
                ? await CreateDbContextWithTransactionAsync(unitOfWork)
                : unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
        }

        /// <summary>
        /// 创建带事务的数据库上下文
        /// </summary>
        protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IUnitOfWork unitOfWork)
        {
            var transactionApiKey = $"SqlSugarCore_{SqlSugarDbContextCreationContext.Current.ConnectionString}";
            var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as SqlSugarTransactionApi;

            if (activeTransaction == null)
            {
                var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
                var transaction = new SqlSugarTransactionApi(dbContext);
                unitOfWork.AddTransactionApi(transactionApiKey, transaction);

                await dbContext.SqlSugarClient.Ado.BeginTranAsync();
                return dbContext;
            }

            return (TDbContext)activeTransaction.GetDbContext();
        }
    }
}
