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

        public virtual async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync(predicate);
        }

        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetFirstAsync(predicate);
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await this.DeleteAsync(predicate);
        }

        public virtual async Task DeleteDirectAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
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

        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetListAsync(predicate);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            return await InsertReturnEntityAsync(entity);
        }

        public virtual async Task InsertManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await InsertRangeAsync(entities.ToList());
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await UpdateAsync(entity);
            return entity;
        }

        public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await UpdateRangeAsync(entities.ToList());
        }

        public virtual async Task DeleteAsync(TEntity entity, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entity);
        }

        public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteAsync(entities.ToList());
        }

        public virtual async Task<List<TEntity>> GetListAsync(bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetListAsync();
        }

        public virtual async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return await this.CountAsync(_=>true);
        }

        public virtual async Task<List<TEntity>> GetPagedListAsync(int skipCount, int maxResultCount, string sorting, bool includeDetails = false, CancellationToken cancellationToken = default)
        {
            return await GetPageListAsync(_ => true, skipCount, maxResultCount);
        }
        #endregion


        #region 内置DB快捷操作
        public virtual async Task<IDeleteable<TEntity>> AsDeleteable()
        {
            return (await GetDbSimpleClientAsync()).AsDeleteable();
        }

        public virtual async Task<IInsertable<TEntity>> AsInsertable(List<TEntity> insertObjs)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObjs);
        }

        public virtual async Task<IInsertable<TEntity>> AsInsertable(TEntity insertObj)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObj);
        }

        public virtual async Task<IInsertable<TEntity>> AsInsertable(TEntity[] insertObjs)
        {
            return (await GetDbSimpleClientAsync()).AsInsertable(insertObjs);
        }

        public virtual async Task<ISugarQueryable<TEntity>> AsQueryable()
        {
            return (await GetDbSimpleClientAsync()).AsQueryable();
        }

        public virtual async Task<ISqlSugarClient> AsSugarClient()
        {
            return (await GetDbSimpleClientAsync()).AsSugarClient();
        }

        public virtual async Task<ITenant> AsTenant()
        {
            return (await GetDbSimpleClientAsync()).AsTenant();
        }

        public virtual async Task<IUpdateable<TEntity>> AsUpdateable(List<TEntity> updateObjs)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObjs);
        }

        public virtual async Task<IUpdateable<TEntity>> AsUpdateable(TEntity updateObj)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObj);
        }

        public virtual async Task<IUpdateable<TEntity>> AsUpdateable()
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable();
        }

        public virtual async Task<IUpdateable<TEntity>> AsUpdateable(TEntity[] updateObjs)
        {
            return (await GetDbSimpleClientAsync()).AsUpdateable(updateObjs);
        }
        #endregion

        #region SimpleClient模块
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).CountAsync(whereExpression);
        }

        public virtual async Task<bool> DeleteAsync(TEntity deleteObj)
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

        public virtual async Task<bool> DeleteAsync(List<TEntity> deleteObjs)
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

        public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
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

        public virtual async Task<bool> DeleteByIdAsync(dynamic id)
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

        public virtual async Task<bool> DeleteByIdsAsync(dynamic[] ids)
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

        public virtual async Task<TEntity> GetByIdAsync(dynamic id)
        {
            return await (await GetDbSimpleClientAsync()).GetByIdAsync(id);
        }



        public virtual async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetFirstAsync(whereExpression);
        }

        public virtual async Task<List<TEntity>> GetListAsync()
        {
            return await (await GetDbSimpleClientAsync()).GetListAsync();
        }

        public virtual async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetListAsync(whereExpression);
        }

        public virtual async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize)
        {
            return await (await GetDbSimpleClientAsync()).GetPageListAsync(whereExpression, new PageModel() { PageIndex = pageNum, PageSize = pageSize });
        }

        public virtual async Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize, Expression<Func<TEntity, object>>? orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return await (await GetDbSimpleClientAsync()).GetPageListAsync(whereExpression, new PageModel { PageIndex = pageNum, PageSize = pageSize }, orderByExpression, orderByType);
        }

        public virtual async Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).GetSingleAsync(whereExpression);
        }

        public virtual async Task<bool> InsertAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertAsync(insertObj);
        }

        public virtual async Task<bool> InsertOrUpdateAsync(TEntity data)
        {
            return await (await GetDbSimpleClientAsync()).InsertOrUpdateAsync(data);
        }

        public virtual async Task<bool> InsertOrUpdateAsync(List<TEntity> datas)
        {
            return await (await GetDbSimpleClientAsync()).InsertOrUpdateAsync(datas);
        }

        public virtual async Task<bool> InsertRangeAsync(List<TEntity> insertObjs)
        {
            return await (await GetDbSimpleClientAsync()).InsertRangeAsync(insertObjs);
        }

        public virtual async Task<long> InsertReturnBigIdentityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnBigIdentityAsync(insertObj);
        }

        public virtual async Task<TEntity> InsertReturnEntityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnEntityAsync(insertObj);
        }

        public virtual async Task<int> InsertReturnIdentityAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnIdentityAsync(insertObj);
        }

        public virtual async Task<long> InsertReturnSnowflakeIdAsync(TEntity insertObj)
        {
            return await (await GetDbSimpleClientAsync()).InsertReturnSnowflakeIdAsync(insertObj);
        }

        public virtual async Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).IsAnyAsync(whereExpression);
        }

        public virtual async Task<bool> UpdateAsync(TEntity updateObj)
        {
            return await (await GetDbSimpleClientAsync()).UpdateAsync(updateObj);
        }

        public virtual async Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression)
        {
            return await (await GetDbSimpleClientAsync()).UpdateAsync(columns, whereExpression);
        }



        public virtual async Task<bool> UpdateRangeAsync(List<TEntity> updateObjs)
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

        public virtual async Task DeleteAsync(TKey id, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteByIdAsync(id);
        }

        public virtual async Task DeleteManyAsync(IEnumerable<TKey> ids, bool autoSave = false, CancellationToken cancellationToken = default)
        {
            await DeleteByIdsAsync(ids.Select(x => (object)x).ToArray());
        }

        public virtual async Task<TEntity?> FindAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id);
        }

        public virtual async Task<TEntity> GetAsync(TKey id, bool includeDetails = true, CancellationToken cancellationToken = default)
        {
            return await GetByIdAsync(id);
        }
    }
}
