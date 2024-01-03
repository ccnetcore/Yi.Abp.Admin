using System.Linq.Expressions;
using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Yi.Framework.SqlSugarCore.Abstractions
{

    public interface ISqlSugarRepository<TEntity>:IRepository<TEntity> where TEntity : class, IEntity,new ()
    {
        ISqlSugarClient _Db { get; }
        ISugarQueryable<TEntity> _DbQueryable { get; }

        Task<ISqlSugarClient> GetDbContextAsync();
        Task<IDeleteable<TEntity>> AsDeleteable();
        Task<IInsertable<TEntity>> AsInsertable(List<TEntity> insertObjs);
        Task<IInsertable<TEntity>> AsInsertable(TEntity insertObj);
        Task<IInsertable<TEntity>> AsInsertable(TEntity[] insertObjs);
        Task<ISugarQueryable<TEntity>> AsQueryable();
        Task<ISqlSugarClient> AsSugarClient();
        Task<ITenant> AsTenant();
        Task<IUpdateable<TEntity>> AsUpdateable(List<TEntity> updateObjs);
        Task<IUpdateable<TEntity>> AsUpdateable(TEntity updateObj);
        Task<IUpdateable<TEntity>> AsUpdateable();
        Task<IUpdateable<TEntity>> AsUpdateable(TEntity[] updateObjs);

        #region 单查
        //单查
        Task<TEntity> GetByIdAsync(dynamic id);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> whereExpression);
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> whereExpression);
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> whereExpression);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression);

        #endregion


        #region 多查
        //多查
        Task<List<TEntity>> GetListAsync();
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> whereExpression);
        #endregion


        #region 分页查
        //分页查
        Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize);
        Task<List<TEntity>> GetPageListAsync(Expression<Func<TEntity, bool>> whereExpression, int pageNum, int pageSize, Expression<Func<TEntity, object>>? orderByExpression = null, OrderByType orderByType = OrderByType.Asc);
        #endregion

        #region 插入
        //插入
        Task<bool> InsertAsync(TEntity insertObj);
        Task<bool> InsertOrUpdateAsync(TEntity data);
        Task<bool> InsertOrUpdateAsync(List<TEntity> datas);
        Task<int> InsertReturnIdentityAsync(TEntity insertObj);
        Task<long> InsertReturnBigIdentityAsync(TEntity insertObj);
        Task<long> InsertReturnSnowflakeIdAsync(TEntity insertObj);
        Task<TEntity> InsertReturnEntityAsync(TEntity insertObj);
        Task<bool> InsertRangeAsync(List<TEntity> insertObjs);
        #endregion


        #region 更新
        //更新
        Task<bool> UpdateAsync(TEntity updateObj);
        Task<bool> UpdateRangeAsync(List<TEntity> updateObjs);
        Task<bool> UpdateAsync(Expression<Func<TEntity, TEntity>> columns, Expression<Func<TEntity, bool>> whereExpression);
        #endregion

        #region 删除
        //删除
        Task<bool> DeleteAsync(TEntity deleteObj);
        Task<bool> DeleteAsync(List<TEntity> deleteObjs);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);
        Task<bool> DeleteByIdAsync(dynamic id);
        Task<bool> DeleteByIdsAsync(dynamic[] ids);
        #endregion

    }


    public interface ISqlSugarRepository<TEntity, TKey> : ISqlSugarRepository<TEntity>,IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>, new()
    { 
    
    
    }
}
