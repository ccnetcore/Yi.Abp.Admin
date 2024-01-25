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
        private readonly string MasterTenantDbDefaultName = DbConnOptions.MasterTenantDbDefaultName;
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
            if (unitOfWork == null || unitOfWork.Options.IsTransactional == false)
            {
                if (ContextInstance.Current is null)
                {
                    ContextInstance.Current = (TDbContext)ServiceProvider.GetRequiredService<ISqlSugarDbContext>();
                }
                var dbContext = (TDbContext)ContextInstance.Current;
                var output = DatabaseChange(dbContext, connectionStringName, connectionString);
                //提高体验，取消工作单元强制性
                //throw new AbpException("A DbContext can only be created inside a unit of work!");
                //如果不启用工作单元，创建一个新的db，不开启事务即可
                return output;
            }




            //lock (_databaseApiLock)
            //{
            //尝试当前工作单元获取db
            var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

            //当前没有db创建一个新的db
            if (databaseApi == null)
            {
                //db根据连接字符串来创建
                databaseApi = new SqlSugarDatabaseApi(
                    CreateDbContextAsync(unitOfWork, connectionStringName, connectionString).Result
                );

                //创建的db加入到当前工作单元中
                unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);

            }
            return (TDbContext)((SqlSugarDatabaseApi)databaseApi).DbContext;
            //}

        }



        protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork, string connectionStringName, string connectionString)
        {
            var creationContext = new SqlSugarDbContextCreationContext(connectionStringName, connectionString);
            //将连接key进行传值
            using (SqlSugarDbContextCreationContext.Use(creationContext))
            {
                var dbContext = await CreateDbContextAsync(unitOfWork);

                //获取到DB之后，对多租户多库进行处理
                var changedDbContext = DatabaseChange(dbContext, connectionStringName, connectionString);
                return changedDbContext;
            }
        }

        protected virtual TDbContext DatabaseChange(TDbContext dbContext, string configId, string connectionString)
        {
            //没有检测到使用多租户功能，默认使用默认库即可
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = dbContext.Options.Url;
                configId = DbConnOptions.TenantDbDefaultName;
            }

            var dbOption = dbContext.Options;
            var db = dbContext.SqlSugarClient.AsTenant();
            //主库的Db切换，当操作的是租户表的时候
            if (CurrentTenant.Name == MasterTenantDbDefaultName)
            {
                //直接切换
                configId = MasterTenantDbDefaultName;
                var conStrOrNull = dbOption.GetDefaultMasterSaasMultiTenancy();
                Volo.Abp.Check.NotNull(conStrOrNull, "租户主库未找到");
                connectionString = conStrOrNull.Url;
            }

            //租户Db的动态切换
            //二级缓存
            var changed = false;
            if (!db.IsAnyConnection(configId))
            {
                var config = _dbConnectionCreator.Build(options =>
                {
                    options.DbType = dbOption.DbType!.Value;
                    options.ConfigId = configId;//设置库的唯一标识
                    options.IsAutoCloseConnection = true;
                    options.ConnectionString = connectionString;
                });
                //添加一个db到当前上下文 (Add部分不线上下文不会共享)
                db.AddConnection(config);
                changed = true;
            }
            var currentDb = db.GetConnection(configId) as ISqlSugarClient;

            //设置Aop
            if (changed)
            {
                _dbConnectionCreator.SetDbAop(currentDb);
            }


            dbContext.SetSqlSugarClient(currentDb);
            return dbContext;
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
