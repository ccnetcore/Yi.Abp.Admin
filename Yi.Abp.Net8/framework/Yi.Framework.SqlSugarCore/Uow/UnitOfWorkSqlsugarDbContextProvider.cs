using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Uow
{
    public class UnitOfWorkSqlsugarDbContextProvider<TDbContext> : ISugarDbContextProvider<TDbContext> where TDbContext : ISqlSugarDbContext
    {
        private readonly ISqlSugarDbConnectionCreator _dbConnectionCreator;
        private readonly string MasterTenantDbDefaultName = DbConnOptions.MasterTenantDbDefaultName;
        public ILogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>> Logger { get; set; }

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

        public virtual async Task<TDbContext> GetDbContextAsync()
        {

            var unitOfWork = UnitOfWorkManager.Current;
            if (unitOfWork == null)
            {
                UnitOfWorkManager.Begin(true);
                unitOfWork = UnitOfWorkManager.Current;
                //取消工作单元强制性
                //throw new AbpException("A DbContext can only be created inside a unit of work!");
            }
            var connectionStringName = ConnectionStrings.DefaultConnectionStringName;
            var connectionString = await ResolveConnectionStringAsync(connectionStringName);
            // var dbContextKey = $"{this.GetType().FullName}_{connectionString}";
            var dbContextKey = "Default";
            var databaseApi = unitOfWork.FindDatabaseApi(dbContextKey);

            if (databaseApi == null)
            {
                databaseApi = new SqlSugarDatabaseApi(
                    await CreateDbContextAsync(unitOfWork, connectionStringName, connectionString)
                );
                unitOfWork.AddDatabaseApi(dbContextKey, databaseApi);

            }
            return (TDbContext)((SqlSugarDatabaseApi)databaseApi).DbContext; ;
        }



        protected virtual async Task<TDbContext> CreateDbContextAsync(IUnitOfWork unitOfWork, string connectionStringName, string connectionString)
        {

            var dbContext = await CreateDbContextAsync(unitOfWork);

            //没有检测到使用多租户功能，默认使用默认库即可
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = dbContext.Options.Url;
                connectionStringName = DbConnOptions.TenantDbDefaultName;
            }

            //获取到DB之后，对多租户多库进行处理
            var changedDbContext = DatabaseChange(dbContext, connectionStringName, connectionString);


            return changedDbContext;
        }

        protected virtual TDbContext DatabaseChange(TDbContext dbContext, string configId, string connectionString)
        {
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
            return unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
            //return unitOfWork.Options.IsTransactional
            //    ? await CreateDbContextWithTransactionAsync(unitOfWork)
            //    : unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
        }
        protected virtual async Task<TDbContext> CreateDbContextWithTransactionAsync(IUnitOfWork unitOfWork)
        {
            var transactionApiKey = $"Sqlsugar_Default" + Guid.NewGuid().ToString();
            var activeTransaction = unitOfWork.FindTransactionApi(transactionApiKey) as SqlSugarTransactionApi;
            //if (activeTransaction==null|| activeTransaction.Equals(default(SqlSugarTransactionApi)))
            //{

            var dbContext = unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
            var transaction = new SqlSugarTransactionApi(
                      dbContext
                  );
            unitOfWork.AddTransactionApi(transactionApiKey, transaction);


            //await Console.Out.WriteLineAsync("开始新的事务");
            // Console.WriteLine(dbContext.SqlSugarClient.ContextID);
            await dbContext.SqlSugarClient.Ado.BeginTranAsync();
            return dbContext;
            //}
            //else
            //{
            //  var db=  activeTransaction.GetDbContext().SqlSugarClient;
            //   // await Console.Out.WriteLineAsync("继续老的事务");
            //   // Console.WriteLine(activeTransaction.DbContext.SqlSugarClient);
            //    await activeTransaction.GetDbContext().SqlSugarClient.Ado.BeginTranAsync();
            //    return (TDbContext)activeTransaction.GetDbContext();
            //}


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
