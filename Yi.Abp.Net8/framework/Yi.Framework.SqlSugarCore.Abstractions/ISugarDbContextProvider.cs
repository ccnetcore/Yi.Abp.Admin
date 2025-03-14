using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// SqlSugar数据库上下文提供者接口
    /// </summary>
    /// <typeparam name="TDbContext">数据库上下文类型</typeparam>
    public interface ISugarDbContextProvider<TDbContext>
        where TDbContext : ISqlSugarDbContext
    {
        /// <summary>
        /// 异步获取数据库上下文实例
        /// </summary>
        /// <returns>数据库上下文实例</returns>
        Task<TDbContext> GetDbContextAsync();
    }
}
