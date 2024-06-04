using Volo.Abp.Data;

namespace Yi.Abp.Web
{
    public class TestOptions
    {
        public ConnectionStrings2 ConnectionStrings { get; set; }=new ConnectionStrings2();

        public AbpDatabaseInfoDictionary2 Databases { get; set; }=new AbpDatabaseInfoDictionary2();
    }

    public class ConnectionStrings2 : Dictionary<string, string?>
    {
    }
    public class AbpDatabaseInfoDictionary2 : Dictionary<string, AbpDatabaseInfo2>
    {
    }

    public class AbpDatabaseInfo2
    {
        internal AbpDatabaseInfo2(string databaseName)
        {
            DatabaseName = databaseName;
            MappedConnections = new HashSet<string>();
        }
        public string DatabaseName { get; }
        public HashSet<string> MappedConnections { get; }
        public bool IsUsedByTenants { get; set; } = true;
    }
}
