using Volo.Abp.DependencyInjection;
using Yi.Framework.Bbs.Domain.Entities.Assignment;

namespace Yi.Framework.Bbs.Domain.Managers.AssignmentProviders;

/// <summary>
/// 任务提供者接口
/// </summary>
public interface IAssignmentProvider : ITransientDependency
{
    /// <summary>
    /// 获取可领取的任务定义，该方法需全部AssignmentProvider去重
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<List<AssignmentDefineAggregateRoot>> GetCanReceiveListAsync(AssignmentContext context);
}