using Volo.Abp.Application.Services;

namespace Yi.Framework.Bbs.Application.Contracts.IServices
{
    /// <summary>
    /// Setting应用抽象
    /// </summary>
    public interface ISettingService : IApplicationService
    {
        /// <summary>
        /// 获取配置标题
        /// </summary>
        /// <returns></returns>
        Task<string> GetTitleAsync();
    }
}
