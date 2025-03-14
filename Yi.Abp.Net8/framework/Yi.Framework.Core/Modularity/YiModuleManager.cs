using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

namespace Yi.Framework.Core.Modularity;

/// <summary>
/// Yi框架模块管理器
/// </summary>
[Dependency(ReplaceServices = true)]
public class YiModuleManager : ModuleManager, IModuleManager, ISingletonDependency
{
    private readonly IModuleContainer _moduleContainer;
    private readonly IEnumerable<IModuleLifecycleContributor> _lifecycleContributors;
    private readonly ILogger<YiModuleManager> _logger;

    /// <summary>
    /// 初始化模块管理器
    /// </summary>
    public YiModuleManager(
        IModuleContainer moduleContainer,
        ILogger<YiModuleManager> logger,
        IOptions<AbpModuleLifecycleOptions> options,
        IServiceProvider serviceProvider) 
        : base(moduleContainer, logger, options, serviceProvider)
    {
        _moduleContainer = moduleContainer;
        _logger = logger;
        _lifecycleContributors = options.Value.Contributors
            .Select(serviceProvider.GetRequiredService)
            .Cast<IModuleLifecycleContributor>()
            .ToArray();
    }

    /// <summary>
    /// 初始化所有模块
    /// </summary>
    /// <param name="context">应用程序初始化上下文</param>
    public override async Task InitializeModulesAsync(ApplicationInitializationContext context)
    {
        _logger.LogDebug("==========模块Initialize初始化统计-跳过0ms模块==========");
        
        var moduleCount = 0;
        var stopwatch = new Stopwatch();
        var totalTime = 0L;

        foreach (var contributor in _lifecycleContributors)
        {
            foreach (var module in _moduleContainer.Modules)
            {
                try
                {
                    stopwatch.Restart();
                    await contributor.InitializeAsync(context, module.Instance);
                    stopwatch.Stop();
                    
                    totalTime += stopwatch.ElapsedMilliseconds;
                    moduleCount++;

                    // 仅记录耗时超过1ms的模块
                    if (stopwatch.ElapsedMilliseconds > 1)
                    {
                        _logger.LogDebug(
                            "耗时-{Time}ms,已加载模块-{ModuleName}", 
                            stopwatch.ElapsedMilliseconds,
                            module.Assembly.GetName().Name);
                    }
                }
                catch (Exception ex)
                {
                    throw new AbpInitializationException(
                        $"模块 {module.Type.AssemblyQualifiedName} 在 {contributor.GetType().FullName} 阶段初始化失败: {ex.Message}",
                        ex);
                }
            }
        }

        _logger.LogInformation(
            "==========【{Count}】个模块初始化执行完毕，总耗时【{Time}ms】==========",
            moduleCount,
            totalTime);
    }
    
}
