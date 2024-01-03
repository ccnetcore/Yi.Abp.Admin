using System.Linq;
using System.Linq.Expressions;
using SqlSugar;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Linq;
using Yi.Framework.Core.Helper;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.SqlSugarCore.Repositories
{
    public class SqlSugarRepository<TEntity> : ISqlSugarRepository<TEntity>, IRepository<TEntity> where TEntity : class, IEntity, new()
    {
        public ISqlSugarClient _Db => GetDbContextAsync().Result;

        public ISugarQueryable<TEntity> _DbQueryable => GetDbContextAsync().Result.Queryable<TEntity>();

        private ISugarDbContextProvider<ISqlSugarDbContext> _sugarDbContextProvider;
        public IAsyncQueryableExecuter AsyncExecuter { get; }

        public bool? IsChangeTrackingEnabled => false;

        public SqlSugarRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider)
        {
            _sugarDbContextProvider = sugarDbContextProvider;
        }

        /// <summary>
        /// 获取DB
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ISqlSugarClient> GetDbContextAsync()
        {

            var db = (await _sugarDbContextProvider.GetDbContextAsync()).SqlSugarClient;
            //await Console.Out.WriteLineAsync("获取的id：" + db.ContextID);
            return db;
        }

        /// <summary>
        /// 获取简单Db
        /// </summary>
        /// <returns></returns>
        public virtual async Task<SimpleClient<TEntity>> GetDbSimpleClientAsync()
        {
            var db = await GetDbContextAsync();
            return new SimpleClient<TEntity>(db);
        }

        #region Abp模块

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync(predicate);
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync(predicate);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await this.DeleteAsync(predicate);
        }

        public async Task DeleteDirectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await this.DeleteAsync(predicate);
        }

        public IQueryable<TEntity> WithDetails()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> WithDetails(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TEntity>> WithDetailsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TEntity>> WithDetailsAsync(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<TEntity>> GetQueryableAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetListAsync(predicate);
        }

        public async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return await InsertReturnEntityAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await InsertRangeAsync(entities.ToList());
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await UpdateAsync(entity);
            return entity;
        }

        public async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await UpdateRangeAsync(entities.ToList());
        }

        public async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity);
        }

        public async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entities.ToList());
        }

        public async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetListAsync();
        }

        public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await this.CountAsync();
        }

        public async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetPageListAsync(_ => true, skipCount, maxResultCount);
        }
        #endregion


        #region 内置DB快捷操作
        public async Task<IDeleteable<TEntity>> AsDeleteable()
        {
            return (await GetDbSimpleClientAsync()).AsDeleteable();
        }

        public async Task<IInsertable<TEntity>> AsInsertable(List<TEntity> insertObjs)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObjs);
        }

        public async Task<IInsertable<TEntity>> AsInsertable(TEntity insertObj)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObj);
        }

        public async Task<IInsertable<TEntity>> AsInsertable(TEntity[] insertObjs)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObjs);
        }

        public async Task<ISugarQueryable<TEntity>> AsQueryable()
        {
            return (await GetDbSimpleClientAsync()).AsQueryable();
        }

        public async Task<ISqlSugarClient> AsSugarClient()
        {
            return (await GetDbSimpleClientAsync()).AsSugarClient();
        }

        public async Task<ITenant> AsTenant()
        {
            return (await GetDbSimpleClientAsync()).AsTenant();
        }

        public async Task<IUpdateable<TEntity>> AsUpdateable(List<TEntity> updateObjs)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObjs);
        }

        public async Task<IUpdateable<TEntity>> AsUpdateable(TEntity updateObj)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObj);
        }

        public async Task<IUpdateable<TEntity>> AsUpdateable()
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable();
        }

        public async Task<IUpdateable<TEntity>> AsUpdateable(TEntity[] updateObjs)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObjs);
        }
        #endregion

        #region SimpleClient模块
        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).CountAsync(whereExpression);
        }

        public async Task<bool> DeleteAsync(TEntity deleteObj)
        {
            if (deleteObj is ISoftDelete)
            {
                ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, deleteObj);
                return await (await GetDbSimpleClientAsync()).UpdateAsync(deleteObj);
            }
            else
            {
                return await (await GetDbSimpleClientAsync()).DeleteAsync(deleteObj);
            }

        }

        public async Task<bool> DeleteAsync(List<TEntity> deleteObjs)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                deleteObjs.ForEach(e => ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, e));
                return await (await GetDbSimpleClientAsync()).UpdateRangeAsync(deleteObjs);
            }
            else
            {
                return await (await GetDbSimpleClientAsync()).DeleteAsync(deleteObjs);
            }
        }

        public async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                return await (await GetDbSimpleClientAsync()).AsUpdateable().SetColumns(nameof(ISoftDelete), true).Where(whereExpression).ExecuteCommandAsync() > 0;
            }
            else
            {
                return await (await GetDbSimpleClientAsync()).DeleteAsync(whereExpression);
            }

        }

        public async Task<bool> DeleteByIdAsync(dynamic id)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var entity = await GetByIdAsync(id);
                //反射赋值
                ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, entity);
                return await UpdateAsync(entity);
            }
            else
            {
                return await (await GetDbSimpleClientAsync()).DeleteByIdAsync(id);
            }
        }

        public async Task<bool> DeleteByIdsAsync(dynamic[] ids)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var simpleClient = (await GetDbSimpleClientAsync());
                var entities = await simpleClient.AsQueryable().In(ids).ToListAsync();
                if (entities.Count == 0)
                {
                    return false;
                }
                //反射赋值
                entities.ForEach(e => ReflexHelper.SetModelValue(nameof(ISoftDelete.IsDeleted), true, e));
                return await UpdateRangeAsync(entities);
            }
            else
            {
                return await (await GetDbSimpleClientAsync()).DeleteByIdAsync(ids);
            }

        }

        public async Task<TEntity> GetByIdAsync(dynamic id)
        {
            return await (await GetDbSimpleClientAsync()).GetByIdAsync(id);
        }



        public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetFirstAsync(whereExpression);
        }

        public async Task<List<TEntity>> GetListAsync()
        {
            return await (await GetDbSimpleClientAsync()).GetListAsync();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetListAsync(whereExpression);
        }

        public async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize)
        {
            return await (await GetDbSimpleClientAsync()).GetPageListAsync(whereExpression, new PageModel() { PageIndex = pageNum, PageSize = pageSize });
        }

        public async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize, Expression<Func<TEntity, object>>? orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return await (await GetDbSimpleClientAsync()).GetPageListAsync(whereExpression, new PageModel { PageIndex = pageNum, PageSize = pageSize }, orderByExpression, orderByType);
        }

        public async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetSingleAsync(whereExpression);
        }

        public async Task<bool> InsertAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertAsync(insertObj);
        }

        public async Task<bool> InsertOrUpdateAsync(TEntity data)
        {
            return await (await GetDbSimpleClientAsync()).InsertOrUpdateAsync(data);
        }

        public async Task<bool> InsertOrUpdateAsync(List<TEntity> datas)
        {
            return await (await GetDbSimpleClientAsync()).InsertOrUpdateAsync(datas);
        }

        public async Task<bool> InsertRangeAsync(List<TEntity> insertObjs)
        {
            return await (await GetDbSimpleClientAsync()).InsertRangeAsync(insertObjs);
        }

        public async Task<long> InsertReturnBigIdentityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnBigIdentityAsync(insertObj);
        }

        public async Task<TEntity> InsertReturnEntityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnEntityAsync(insertObj);
        }

        public async Task<int> InsertReturnIdentityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnIdentityAsync(insertObj);
        }

        public async Task<long> InsertReturnSnowflakeIdAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnSnowflakeIdAsync(insertObj);
        }

        public async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).IsAnyAsync(whereExpression);
        }

        public async Task<bool> UpdateAsync(TEntity updateObj)
        {
            return await (await GetDbSimpleClientAsync()).UpdateAsync(updateObj);
        }

        public async Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).UpdateAsync(columns, whereExpression);
        }



        public async Task<bool> UpdateRangeAsync(List<TEntity> updateObjs)
        {
            return await (await GetDbSimpleClientAsync()).UpdateRangeAsync(updateObjs);
        }

        #endregion
    }

    public class SqlSugarRepository<TEntity, TKey> : SqlSugarRepository<TEntity>, ISqlSugarRepository<TEntity, TKey>, IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    {
        public SqlSugarRepository(ISugarDbContextProvider<ISqlSugarDbContext> sugarDbContextProvider) : base(sugarDbContextProvider)
        {
        }

        public async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteByIdAsync(id);
        }

        public async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteByIdsAsync(ids.Select(x => (object)x).ToArray());
        }

        public async Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id);
        }

        public async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id);
        }
    }
}
