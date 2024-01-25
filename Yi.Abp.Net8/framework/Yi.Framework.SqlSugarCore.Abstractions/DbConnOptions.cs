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


        /// <summary>
        /// 默认租户库连接,如果不填，那就是默认库的地址
        /// </summary>
        public string? MasterSaasMultiTenancyUrl { get; set; }


        /// <summary>
        /// Saas租户连接
        /// </summary>
        public List<SaasMultiTenancyOptions>? SaasMultiTenancy { get; set; }

        public static string MasterTenantDbDefaultName = "Master";
        public static string TenantDbDefaultName = "Default";

        public SaasMultiTenancyOptions GetDefaultSaasMultiTenancy()
        {
            return new SaasMultiTenancyOptions { Name = TenantDbDefaultName, Url = Url };
        }
        public SaasMultiTenancyOptions? GetDefaultMasterSaasMultiTenancy()
        {
            if (EnabledSaasMultiTenancy == false)
            {
                return null;
            }
            if (string.IsNullOrEmpty(MasterSaasMultiTenancyUrl))
            {

                return new SaasMultiTenancyOptions { Name = MasterTenantDbDefaultName, Url = Url };
            }
            else
            {
                return new SaasMultiTenancyOptions()
                {
                    Name = MasterTenantDbDefaultName,
                    Url = MasterSaasMultiTenancyUrl
                };
            }
        }
    }

    public class SaasMultiTenancyOptions
    {
        /// <summary>
        /// 租户名称标识
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 连接Url
        /// </summary>
        public string Url { get; set; }
    }
}
