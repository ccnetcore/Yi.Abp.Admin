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
        private readonly ISqlSugarDbConnectionCreator _dbConnectionCreator;

        public ILogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>> Logger { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        private static AsyncLocalDbContextAccessor ContextInstance => AsyncLocalDbContextAccessor.Instance;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IConnectionStringResolver ConnectionStringResolver;
        protected readonly ICancellationTokenProvider CancellationTokenProvider;
        protected readonly ICurrentTenant CurrentTenant;

        public UnitOfWorkSqlsugarDbContextProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            ICancellationTokenProvider cancellationTokenProvider,
            ICurrentTenant currentTenant,
            ISqlSugarDbConnectionCreator dbConnectionCreator
        )
        {
            UnitOfWorkManager = unitOfWorkManager;
            ConnectionStringResolver = connectionStringResolver;
            CancellationTokenProvider = cancellationTokenProvider;
            CurrentTenant = currentTenant;
            Logger = NullLogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>>.Instance;
            _dbConnectionCreator = dbConnectionCreator;
        }

        //private static object _databaseApiLock = new object();
        public virtual async Task<TDbContext> GetDbContextAsync()
        {

            var connectionStringName = ConnectionStrings.DefaultConnectionStringName;

            //获取当前连接字符串，未多租户时，默认为空
            var connectionString = await ResolveConnectionStringAsync(connectionStringName);
            var dbContextKey = $"{this.GetType().FullName}_{connectionString}";


            var unitOfWork = UnitOfWorkManager.Current;
            if (unitOfWork == null /*|| unitOfWork.Options.IsTransactional == false*/)
            {
                var dbContext = (TDbContext)ServiceProvider.GetRequiredService<ISqlSugarDbContext>();
                //提高体验，取消工作单元强制性
                //throw new AbpException("A DbContext can only be created inside a unit of work!");
                //如果不启用工作单元，创建一个新的db，不开启事务即可
                return dbContext;
            }





            //尝试当前工作单元获取db
            var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

            //当前没有db创建一个新的db
            if (databaseApi == null)
            {
                //db根据连接字符串来创建
                databaseApi = new SqlSugarDatabaseApi(
                  await CreateDbContextAsync(unitOfWork, connectionStringName, connectionString)
                );

                //await Console.Out.WriteLineAsync(">>>----------------实例化了db"+ ((SqlSugarDatabaseApi)databaseApi).DbContext.SqlSugarClient.ContextID.ToString());
                //创建的db加入到当前工作单元中
                unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);

            }
            return (TDbContext)((SqlSugarDatabaseApi)databaseApi).DbContext;
        }



        protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork, string connectionStringName, string connectionString)
        {
            var creationContext = new SqlSugarDbContextCreationContext(connectionStringName, connectionString);
            //将连接key进行传值
            using (SqlSugarDbContextCreationContext.Use(creationContext))
            {
                var dbContext = await CreateDbContextAsync(unitOfWork);
                return dbContext;
            }
        }

        protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Options.IsTransactional
                ? await CreateDbContextWithTransactionAsync(unitOfWork)
                : unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
        }
        protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IUnitOfWork unitOfWork)
        {
            //事务key
            var transactionApiKey = $"SqlsugarCore_{SqlSugarDbContextCreationContext.Current.ConnectionString}";

            //尝试查找事务
            var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as SqlSugarTransactionApi;

            //该db还没有进行开启事务
            if (activeTransaction == null)
            {
                //获取到db添加事务即可
                var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
                var transaction = new SqlSugarTransactionApi(
                          dbContext
                      );
                unitOfWork.AddTransactionApi(transactionApiKey, transaction);

                await dbContext.SqlSugarClient.Ado.BeginTranAsync();
                return dbContext;
            }
            else
            {
                return (TDbContext)activeTransaction.GetDbContext();
            }


        }


        protected virtual async Task<string> ResolveConnectionStringAsync(string connectionStringName)
        {
            if (typeof(TDbContext).IsDefined(typeof(IgnoreMultiTenancyAttribute), false))
            {
                using (CurrentTenant.Change(null))
                {
                    return await ConnectionStringResolver.ResolveAsync(connectionStringName);
                }
            }

            return await ConnectionStringResolver.ResolveAsync(connectionStringName);
        }

    }
}
