namespace Yi.Framework.Bbs.Domain.Shared.Caches;

public class AccessLogCacheItem
{
    public AccessLogCacheItem(long number)
    {
        Number = number;
    }

    public long Number { get; set; }
    public DateTime LastModificationTime { get; set; }=DateTime.Now;
    
    public DateTime LastInsertTime { get; set; }=DateTime.Now;
}

public class AccessLogCacheConst
{

    public const string Key = "AccessLog";
}