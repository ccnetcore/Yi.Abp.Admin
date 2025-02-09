using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;

namespace Yi.Framework.WeChat.MiniProgram.Token;

internal class CacheMiniProgramToken : DefaultMinProgramToken, IMiniProgramToken
{
    private IDistributedCache<string> _cache;
    private const string CacheKey = "MiniProgramToken";

    public CacheMiniProgramToken(IOptions<WeChatMiniProgramOptions> options, IDistributedCache<string> cache) :
        base(options)
    {
        _cache = cache;
    }

    public async Task<string> GetTokenAsync()
    {
        return await _cache.GetOrAddAsync("MiniProgramToken", async () => { return await base.GetTokenAsync(); }, () =>
        {
            return new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) - TimeSpan.FromMinutes(1)
            };
        });
    }
}