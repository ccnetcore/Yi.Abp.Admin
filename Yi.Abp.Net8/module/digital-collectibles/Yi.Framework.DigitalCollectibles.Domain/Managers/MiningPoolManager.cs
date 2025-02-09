using FreeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using Yi.Framework.DigitalCollectibles.Domain.Dtos;
using Yi.Framework.DigitalCollectibles.Domain.Entities;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Consts;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Enums;
using Yi.Framework.DigitalCollectibles.Domain.Shared.Etos;
using Yi.Framework.SettingManagement.Domain;
using Yi.Framework.SqlSugarCore.Abstractions;

namespace Yi.Framework.DigitalCollectibles.Domain.Managers;

/// <summary>
/// 矿池领域服务
/// 处理矿池相关业务，例如挖矿等
/// </summary>
public class MiningPoolManager : DomainService
{
    private readonly ISqlSugarRepository<CollectiblesAggregateRoot> _collectiblesRepository;
    private readonly ISqlSugarRepository<OnHookAggregateRoot> _onHookRepository;
    private readonly ISettingProvider _settingProvider;
    private readonly IDistributedCache<MiningPoolContent?> _miningPoolCache;
    private readonly IDistributedCache<UserMiningLimitCacheDto?> _userMiningLimitCache;
    private readonly ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> _userStoreRepository;
    private readonly ILogger<MiningPoolManager> _logger;
    private IRedisClient RedisClient => LazyServiceProvider.LazyGetRequiredService<IRedisClient>();
    private ILocalEventBus LocalEventBus => LazyServiceProvider.LazyGetRequiredService<ILocalEventBus>();

    public MiningPoolManager(ISettingProvider settingProvider, IDistributedCache<MiningPoolContent> miningPoolCache,
        ISqlSugarRepository<CollectiblesAggregateRoot> collectiblesRepository,
        ISqlSugarRepository<OnHookAggregateRoot> onHookRepository,
        ISqlSugarRepository<CollectiblesUserStoreAggregateRoot> userStoreRepository,
        IDistributedCache<UserMiningLimitCacheDto> userMiningLimitCache, ILogger<MiningPoolManager> logger)
    {
        _settingProvider = settingProvider;
        this._miningPoolCache = miningPoolCache;
        _collectiblesRepository = collectiblesRepository;
        _onHookRepository = onHookRepository;
        _userStoreRepository = userStoreRepository;
        _userMiningLimitCache = userMiningLimitCache;
        _logger = logger;
    }

    /// <summary>
    /// 扣减矿池
    /// </summary>
    /// <param name="rarity">矿物等级</param>
    public async Task DeductionPoolAsync(RarityEnum rarity)
    {
        var pool = await GetMiningPoolContentAsync();
        switch (rarity)
        {
            case RarityEnum.Ordinary:
                pool.I0_OrdinaryNumber -= 1;
                break;
            case RarityEnum.Senior:
                pool.I1_SeniorNumber -= 1;
                break;
            case RarityEnum.Rare:
                pool.I2_RareNumber -= 1;
                break;
            case RarityEnum.Gem:
                pool.I3_GemNumber -= 1;
                break;
            case RarityEnum.Legend:
                pool.I4_LegendNumber -= 1;
                break;
        }

        //重新设置
        await SetMiningPoolAsync(pool);
    }


    /// <summary>
    /// 每次挖矿概率，每天根据特定算法计算
    /// </summary>
    private decimal CurrentMiningProbability => AsyncHelper.RunSync(async () =>
    {
        return await ComputeMiningProbabilityAsync();
    });

    public async Task<MiningPoolContent?> GetMiningPoolContentAsync()
    {
        var pool = await _miningPoolCache.GetAsync(MiningCacheConst.MiningPoolContent);
        return pool;
    }

    public async Task GetOnHookAsync(Guid userId)
    {
        var onHook = await _onHookRepository._DbQueryable.Where(x => x.UserId == userId)
            .Where(x => x.IsActive == true)
            .Where(x => x.EndTime > DateTime.Now)
            .FirstAsync();

        if (onHook is not null)
        {
            throw new UserFriendlyException($"当前你正在进行自动挂机，结束时间:{onHook.EndTime.Value.ToString("MM月dd日HH分mm秒")})");
        }

        await _onHookRepository.InsertAsync(new OnHookAggregateRoot(userId, 24));
    }

    /// <summary>
    /// 校验挖矿限制
    /// </summary>
    /// <param name="userId"></param>
    private async Task VerifyMiningLimitAsync(Guid userId)
    {
        //每天最大次数限制
        var miningMaxLimit = int.Parse(await _settingProvider.GetOrNullAsync("MiningMaxLimit"));
        //每次间隔
        var miningMinIntervalSeconds = int.Parse(await _settingProvider.GetOrNullAsync("MiningMinIntervalSeconds"));

        var currentNumber = 1;

        //根据用户进行上锁
        if (await RedisClient.SetNxAsync($"UserMiningLimitLock:{userId}", true,
                TimeSpan.FromSeconds(miningMinIntervalSeconds)))
        {
            var userLimit = await _userMiningLimitCache.GetAsync($"{MiningCacheConst.UserMiningLimit}:{userId}");

            if (userLimit is not null)
            {
                //符合条件，成功挖矿
                if (userLimit.Number < miningMaxLimit)
                {
                    currentNumber = userLimit.Number + 1;
                }
                else
                {
                    throw new UserFriendlyException($"失败，你已达当轮矿池最大限制，给其他人留点吧");
                }
            }

            //没有缓存过，必定成功，直接走
            await _userMiningLimitCache.SetAsync($"{MiningCacheConst.UserMiningLimit}:{userId}",
                new UserMiningLimitCacheDto
                {
                    Number = currentNumber,
                    LastMiningTime = DateTime.Now,
                },
                new DistributedCacheEntryOptions()
                {
                    //虽然新增的是一天，但是每次刷新是早上10点，矿池刷新时，还需要清除限制
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                });
            return;
        }

        //已上过锁，并且没有到限制时间，必定失败
        //不符合条件，直接走
        throw new UserFriendlyException($"失败，你都挖的冒烟了，请稍后再挖");
    }

    /// <summary>
    /// 用户进行一次挖矿
    /// </summary>
    /// <returns></returns>
    public async Task<MiningPoolResult> MiningAsync(Guid userId)
    {
        //检验限制
        await VerifyMiningLimitAsync(userId);

        var result = new MiningPoolResult();
        //从矿池中开挖,判断矿池是否还有矿
        //根据挖矿概率，进行挖掘
        //挖到了放到用户仓库即可

        //如果概率是挖到了矿，再从矿物中随机选择一个稀有度，再在当前稀有度中的矿物列表，随机选择一个具体的矿物
        var pool = await GetMiningPoolContentAsync();
        if (pool is null || pool.TotalNumber == 0)
        {
            throw new UserFriendlyException($"失败，矿池已经被掏空了，请等矿池刷新后再来");
        }

        // 生成一个 0 到 1 之间的随机数
        double randomValue = new Random().NextDouble();
        //如果随机的概率在当前概率内，成功
        if (randomValue.To<decimal>() < CurrentMiningProbability)
        {
            //成功后在藏品中根据稀有度概率必定获取一个
            var probabilityArray = RarityEnumExtensions.GetProbabilityArray();
            var index = GetRandomIndex(probabilityArray);
            var rarityType = (RarityEnum)Enum.GetValues(typeof(RarityEnum)).GetValue(index)!;
            var collectiblesList =
                await _collectiblesRepository._DbQueryable.Where(x => x.Rarity == rarityType).ToListAsync();
            //当前等级的矿物没有设置
            if (collectiblesList.Count == 0)
            {
                throw new UserFriendlyException($"可惜！差一点就挖到了");
            }

            bool poolState = true;
            switch (rarityType)
            {
                case RarityEnum.Ordinary:
                    if (pool.I0_OrdinaryNumber <= 0)
                    {
                        poolState = false;
                    }

                    break;
                case RarityEnum.Senior:
                    if (pool.I1_SeniorNumber <= 0)
                    {
                        poolState = false;
                    }

                    break;
                case RarityEnum.Rare:
                    if (pool.I2_RareNumber <= 0)
                    {
                        poolState = false;
                    }

                    break;
                case RarityEnum.Gem:
                    if (pool.I3_GemNumber <= 0)
                    {
                        poolState = false;
                    }

                    break;
                case RarityEnum.Legend:
                    if (pool.I4_LegendNumber <= 0)
                    {
                        poolState = false;
                    }

                    break;
            }

            if (poolState == false)
            {
                throw new UserFriendlyException($"超级可惜！真的真的只差最后一点就挖到了");
            }


            int randomIndex = new Random().Next(collectiblesList.Count);
            var currentCollectibles = collectiblesList[randomIndex];

            result.Collectibles = currentCollectibles;

            await LocalEventBus.PublishAsync(new SuccessMiningEto
            {
                CollectiblesId = currentCollectibles.Id,
                UserId = userId
            }, false);

            return result;
        }

        throw new UserFriendlyException($"恭喜！空空如也，挖了个寂寞");
    }

    /// <summary>
    /// 挂机挖矿
    /// </summary>
    public async Task OnHookMiningAsync()
    {
        //获取当前激活的挂机挖矿
        var currentOnHook = await _onHookRepository._DbQueryable.Where(x => x.IsActive == true)
            .Where(x => x.EndTime <= DateTime.Now).ToListAsync();

        //根据用户对挂机卡hash关系
        var userOnHookDic = currentOnHook.GroupBy(x => x.UserId).ToDictionary(x => x.Key, y => y.First());
        foreach (var onHookItem in userOnHookDic)
        {
            try
            {
                await MiningAsync(onHookItem.Value.UserId);
            }
            catch (UserFriendlyException e)
            {
                _logger.LogInformation($"自动挖矿-{onHookItem.Value.UserId},{e.Message}");
            }
        }
    }

    private int GetRandomIndex(decimal[] probabilities)
    {
        // 生成0到1之间的随机数
        Random random = new Random();
        decimal randomValue = random.NextDouble().To<decimal>();

        decimal cumulativeProbability = 0.0m;

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];

            // 判断随机数是否小于当前的累积概率
            if (randomValue < cumulativeProbability)
            {
                return i; // 返回中标的索引
            }
        }

        //剩余情况都是普通
        return 0;
    }

    /// <summary>
    /// 计算当前挖矿概率
    /// </summary>
    /// <returns></returns>
    public async Task<decimal> ComputeMiningProbabilityAsync()
    {
        //当前的挖矿期望：当天的所有藏品能被刚好挖完

        //保底概率：最大不能高过一个值，百分之10
        //手动挖矿：1天可挖10次，每次至少间隔6秒
        //自动挖矿：1天可以挖24次  每次间隔60分钟（需要使用自动挖矿卡）
        //可影响因素：剩余手动挖矿次数+剩余自动挖矿次数

        //简单模式，1/15
        var miningMaxLimit = decimal.Parse(await _settingProvider.GetOrNullAsync("MiningMinProbability"));
        return miningMaxLimit;
    }

    /// <summary>
    /// 刷新矿池
    /// </summary>
    /// <returns></returns>
    public async Task RefreshMiningPoolAsync()
    {
        //获取当前最大的限制
        // var maximumPoolLimit = int.Parse(await _settingProvider.GetOrNullAsync("MaxPoolLimit"));
        var poolData = (await _settingProvider.GetOrNullAsync("PoolData")).Split(',').Select(x => int.Parse(x))
            .ToList();
        DateTime startTime = DateTime.Today.AddHours(10);
        DateTime endTime = startTime.AddDays(1);
        // var probabilityValues = RarityEnumExtensions.GetProbabilityArray();
        // var result = GenerateDistribution(maximumPoolLimit, probabilityValues);

        //根据配置，将不同比例的矿，塞入矿池,
        //矿池，交给redis

        await SetMiningPoolAsync(new MiningPoolContent(startTime, endTime)
        {
            I0_OrdinaryNumber = poolData[0],
            I1_SeniorNumber = poolData[1],
            I2_RareNumber = poolData[2],
            I3_GemNumber = poolData[3],
            I4_LegendNumber = poolData[4]
        });
    }

    private async Task SetMiningPoolAsync(MiningPoolContent content)
    {
        await _miningPoolCache.SetAsync(MiningCacheConst.MiningPoolContent, content
            , new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = content.EndTime
            });
    }

    /// <summary>
    /// 缓存前缀
    /// </summary>
    private string CacheKeyPrefix => LazyServiceProvider.LazyGetRequiredService<IOptions<AbpDistributedCacheOptions>>()
        .Value.KeyPrefix;

    /// <summary>
    /// 刷新用户挖矿限制
    /// </summary>
    public async Task RefreshMiningUserLimitAsync()
    {
        var needKeys = await RedisClient.KeysAsync($"{CacheKeyPrefix}{MiningCacheConst.UserMiningLimit}*");
        foreach (var needKey in needKeys)
        {
            await RedisClient.DelAsync(needKey);
        }
    }

    /// <summary>
    /// 根据概率生成给对应稀有度藏品
    /// </summary>
    /// <param name="totalCount"></param>
    /// <param name="probabilities"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private int[] GenerateDistribution(int totalCount, decimal[] probabilities)
    {
        int[] counts = new int[probabilities.Length];
        decimal totalProbability = 0;

        // 计算概率总和，确保为 1
        foreach (var prob in probabilities)
        {
            totalProbability += prob;
        }

        // 检查概率之和是否为 1
        if (totalProbability < 0.99m || totalProbability > 1.01m)
        {
            throw new ArgumentException("概率总和必须接近1");
        }

        // 生成分布
        for (int i = 0; i < probabilities.Length; i++)
        {
            counts[i] = (int)(totalCount * probabilities[i]);
        }

        // 处理可能因精度问题导致的总数不足
        int sum = 0;
        foreach (var count in counts)
        {
            sum += count;
        }

        int difference = totalCount - sum;

        // 将差值分配给概率最大的一项
        if (difference > 0)
        {
            int maxIndex = Array.IndexOf(counts, Math.Max(counts[0], counts[1]));
            counts[maxIndex] += difference;
        }

        return counts;
    }
}