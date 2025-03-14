using System.Linq.Expressions;
using SqlSugar;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// SqlSugar仓储接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface ISqlSugarRepository<TEntity> : IRepository<TEntity>, IUnitOfWorkEnabled 
        where TEntity : class, IEntity, new()
    {
        #region 数据库访问器

        /// <summary>
        /// 获取SqlSugar客户端实例
        /// </summary>
        ISqlSugarClient _Db { get; }

        /// <summary>
        /// 获取查询构造器
        /// </summary>
        ISugarQueryable<TEntity> _DbQueryable { get; }

        /// <summary>
        /// 异步获取数据库上下文
        /// </summary>
        Task<ISqlSugarClient> GetDbContextAsync();

        /// <summary>
        /// 获取删除操作构造器
        /// </summary>
        Task<IDeleteable<TEntity>> AsDeleteable();

        /// <summary>
        /// 获取插入操作构造器
        /// </summary>
        Task<IInsertable<TEntity>> AsInsertable(TEntity entity);

        /// <summary>
        /// 获取批量插入操作构造器
        /// </summary>
        Task<IInsertable<TEntity>> AsInsertable(List<TEntity> entities);

        /// <summary>
        /// 获取查询构造器
        /// </summary>
        Task<ISugarQueryable<TEntity>> AsQueryable();

        /// <summary>
        /// 获取SqlSugar客户端
        /// </summary>
        Task<ISqlSugarClient> AsSugarClient();

        /// <summary>
        /// 获取租户操作接口
        /// </summary>
        Task<ITenant> AsTenant();

        /// <summary>
        /// 获取更新操作构造器
        /// </summary>
        Task<IUpdateable<TEntity>> AsUpdateable();

        /// <summary>
        /// 获取实体更新操作构造器
        /// </summary>
        Task<IUpdateable<TEntity>> AsUpdateable(TEntity entity);

        /// <summary>
        /// 获取批量更新操作构造器
        /// </summary>
        Task<IUpdateable<TEntity>> AsUpdateable(List<TEntity> entities);

        #endregion

        #region 查询操作

        /// <summary>
        /// 根据主键获取实体
        /// </summary>
        Task<TEntity> GetByIdAsync(dynamic id);

        /// <summary>
        /// 获取满足条件的单个实体
        /// </summary>
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取满足条件的第一个实体
        /// </summary>
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 判断是否存在满足条件的实体
        /// </summary>
        Task<bool> IsAnyAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取满足条件的实体数量
        /// </summary>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<List<TEntity>> GetListAsync();

        /// <summary>
        /// 获取满足条件的所有实体
        /// </summary>
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region 分页查询

        /// <summary>
        /// 获取分页数据
        /// </summary>
        Task<List<TEntity>> GetPageListAsync(
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize);

        /// <summary>
        /// 获取排序的分页数据
        /// </summary>
        Task<List<TEntity>> GetPageListAsync(
            Expression<Func<TEntity, bool>> predicate,
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, object>>? orderByExpression = null,
            OrderByType orderByType = OrderByType.Asc);

        #endregion

        #region 插入操作

        /// <summary>
        /// 插入实体
        /// </summary>
        Task<bool> InsertAsync(TEntity entity);

        /// <summary>
        /// 插入或更新实体
        /// </summary>
        Task<bool> InsertOrUpdateAsync(TEntity entity);

        /// <summary>
        /// 批量插入或更新实体
        /// </summary>
        Task<bool> InsertOrUpdateAsync(List<TEntity> entities);

        /// <summary>
        /// 插入实体并返回自增主键
        /// </summary>
        Task<int> InsertReturnIdentityAsync(TEntity entity);

        /// <summary>
        /// 插入实体并返回长整型自增主键
        /// </summary>
        Task<long> InsertReturnBigIdentityAsync(TEntity entity);

        /// <summary>
        /// 插入实体并返回雪花ID
        /// </summary>
        Task<long> InsertReturnSnowflakeIdAsync(TEntity entity);

        /// <summary>
        /// 插入实体并返回实体
        /// </summary>
        Task<TEntity> InsertReturnEntityAsync(TEntity entity);

        /// <summary>
        /// 批量插入实体
        /// </summary>
        Task<bool> InsertRangeAsync(List<TEntity> entities);

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        Task<bool> UpdateAsync(TEntity entity);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        Task<bool> UpdateRangeAsync(List<TEntity> entities);

        /// <summary>
        /// 条件更新指定列
        /// </summary>
        Task<bool> UpdateAsync(
            Expression<Func<TEntity, TEntity>> columns,
            Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region 删除操作

        /// <summary>
        /// 删除实体
        /// </summary>
        Task<bool> DeleteAsync(TEntity entity);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        Task<bool> DeleteAsync(List<TEntity> entities);

        /// <summary>
        /// 条件删除
        /// </summary>
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 根据主键删除
        /// </summary>
        Task<bool> DeleteByIdAsync(dynamic id);

        /// <summary>
        /// 根据主键批量删除
        /// </summary>
        Task<bool> DeleteByIdsAsync(dynamic[] ids);

        #endregion
    }

    /// <summary>
    /// SqlSugar仓储接口(带主键)
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public interface ISqlSugarRepository<TEntity, TKey> : 
        ISqlSugarRepository<TEntity>,
        IRepository<TEntity, TKey> 
        where TEntity : class, IEntity<TKey>, new()
    {
    }
}
