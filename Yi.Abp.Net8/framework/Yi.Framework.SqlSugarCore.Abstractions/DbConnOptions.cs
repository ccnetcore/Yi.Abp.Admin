using SqlSugar;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    public class DbConnOptions
    {
        /// <summary>
        /// 连接字符串(如果开启多租户，也就是默认库了)，必填
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// 开启种子数据
        /// </summary>
        public bool EnabledDbSeed { get; set; } = false;



        /// <summary>
        /// 开启codefirst
        /// </summary>
        public bool EnabledCodeFirst { get; set; } = false;

        /// <summary>
        /// 开启sql日志
        /// </summary>
        public bool EnabledSqlLog { get; set; } = true;

        /// <summary>
        /// 实体程序集
        /// </summary>
        public List<string>? EntityAssembly { get; set; }

        /// <summary>
        /// 开启读写分离
        /// </summary>
        public bool EnabledReadWrite { get; set; } = false;

        /// <summary>
        /// 读写分离
        /// </summary>
        public List<string>? ReadUrl { get; set; }

        /// <summary>
        /// 开启Saas多租户
        /// </summary>
        public bool EnabledSaasMultiTenancy { get; set; } = false;

    }
}
