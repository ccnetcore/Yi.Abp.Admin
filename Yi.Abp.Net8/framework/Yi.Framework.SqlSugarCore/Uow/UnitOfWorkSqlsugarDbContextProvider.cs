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

        public ILogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>> Logger { get; set; }

        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IConnectionStringResolver ConnectionStringResolver;
        protected readonly ICancellationTokenProvider CancellationTokenProvider;
        protected readonly ICurrentTenant CurrentTenant;

        public UnitOfWorkSqlsugarDbContextProvider(
            IUnitOfWorkManager unitOfWorkManager,
            IConnectionStringResolver connectionStringResolver,
            ICancellationTokenProvider cancellationTokenProvider,
            ICurrentTenant currentTenant
        )
        {
            UnitOfWorkManager = unitOfWorkManager;
            ConnectionStringResolver = connectionStringResolver;
            CancellationTokenProvider = cancellationTokenProvider;
            CurrentTenant = currentTenant;
            Logger = NullLogger<UnitOfWorkSqlsugarDbContextProvider<TDbContext>>.Instance;
        }

        public virtual async Task<TDbContext> GetDbContextAsync()
        {

            var unitOfWork = UnitOfWorkManager.Current;
            if (unitOfWork == null)
            {
                throw new AbpException("A DbContext can only be created inside a unit of work!");
            }
            //var sss= unitOfWork.ServiceProvider.GetRequiredService<TDbContext>();
            //Console.WriteLine("反户的:"+sss.SqlSugarClient.ContextID);
            //return sss;


            var connectionStringName = "Default";
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
           // Console.WriteLine("111111：" + dbContext.SqlSugarClient.ContextID);
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
            var transactionApiKey = $"Sqlsugar_Default"+Guid.NewGuid().ToString();
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
