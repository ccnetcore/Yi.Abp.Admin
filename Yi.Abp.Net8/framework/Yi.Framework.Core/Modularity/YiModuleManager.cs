using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Modularity;

namespace Yi.Framework.Core.Modularity;

[Dependency(ReplaceServices =true)]
public class YiModuleManager : ModuleManager, IModuleManager, ISingletonDependency
{
    private readonly IModuleContainer _moduleContainer;
    private readonly IEnumerable<IModuleLifecycleContributor> _lifecycleContributors;
    private readonly ILogger<YiModuleManager> _logger;

    public YiModuleManager(IModuleContainer moduleContainer, ILogger<YiModuleManager> logger, IOptions<AbpModuleLifecycleOptions> options, IServiceProvider serviceProvider) : base(moduleContainer, logger, options, serviceProvider)
    {
        _moduleContainer = moduleContainer;
        _logger = logger;
        _lifecycleContributors = options.Value.Contributors.Select(serviceProvider.GetRequiredService).Cast<IModuleLifecycleContributor>().ToArray();
    }

    public override async Task InitializeModulesAsync(ApplicationInitializationContext context)
    {

        _logger.LogDebug("==========模块Initialize初始化统计-跳过0ms模块==========");
        var total = 0;
        var watch =new Stopwatch();
        long totalTime = 0;
        foreach (var contributor in _lifecycleContributors)
        {
            foreach (var module in _moduleContainer.Modules)
            {
                try
                {
                    watch.Restart();
                    await contributor.InitializeAsync(context, module.Instance);
                    watch.Stop();
                    totalTime += watch.ElapsedMilliseconds;
                    total++;
                    if (watch.ElapsedMilliseconds > 1)
                    {
                        _logger.LogDebug($"耗时-{watch.ElapsedMilliseconds}ms,已加载模块-{module.Assembly.GetName().Name}");
                    }
                   
                }
                catch (Exception ex)
                {
                    throw new AbpInitializationException($"An error occurred during the initialize {contributor.GetType().FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                }
            }
        }
     
        _logger.LogInformation($"==========【{total}】个模块初始化执行完毕，总耗时【{totalTime}ms】==========");
    }
    
}
