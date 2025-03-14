using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using Volo.Abp.DependencyInjection;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// SqlSugar数据库上下文接口
    /// </summary>
    public interface ISqlSugarDbContext
    {
        /// <summary>
        /// 获取SqlSugar客户端实例
        /// </summary>
        ISqlSugarClient SqlSugarClient { get; }
        
        /// <summary>
        /// 执行数据库备份
        /// </summary>
        void BackupDataBase();
    }
}
