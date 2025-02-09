namespace Yi.Framework.DigitalCollectibles.Domain.Shared.Caches;

public class WeChatNoticeCacheItem
{
    public WeChatNoticeCacheItem(bool isSubscribe)
    {
        IsSubscribe = isSubscribe;
    }

    public bool IsSubscribe { get; set; }
}