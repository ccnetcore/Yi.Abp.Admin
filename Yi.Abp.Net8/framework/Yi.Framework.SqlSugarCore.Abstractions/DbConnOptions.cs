using SqlSugar;
using ArgumentException = System.ArgumentException;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// 数据库连接配置选项
    /// </summary>
    public class DbConnOptions
    {
        /// <summary>
        /// 主数据库连接字符串
        /// 如果开启多租户，此为默认租户数据库
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// 是否启用种子数据初始化
        /// </summary>
        public bool EnabledDbSeed { get; set; } = false;

        /// <summary>
        /// 是否启用驼峰命名转下划线命名
        /// </summary>
        public bool EnableUnderLine { get; set; } = false;

        /// <summary>
        /// 是否启用Code First模式
        /// </summary>
        public bool EnabledCodeFirst { get; set; } = false;

        /// <summary>
        /// 是否启用SQL日志记录
        /// </summary>
        public bool EnabledSqlLog { get; set; } = true;

        /// <summary>
        /// 实体类所在程序集名称列表
        /// </summary>
        public List<string>? EntityAssembly { get; set; }

        /// <summary>
        /// 是否启用读写分离
        /// </summary>
        public bool EnabledReadWrite { get; set; } = false;

        /// <summary>
        /// 只读数据库连接字符串列表
        /// </summary>
        public List<string>? ReadUrl { get; set; }

        /// <summary>
        /// 是否启用SaaS多租户
        /// </summary>
        public bool EnabledSaasMultiTenancy { get; set; } = false;

        /// <summary>
        /// 是否开启更新并发乐观锁
        /// </summary>
        public bool EnabledConcurrencyException { get;set; } = false;
    }
}