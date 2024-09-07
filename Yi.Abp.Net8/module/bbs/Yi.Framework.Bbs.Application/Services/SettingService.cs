using Volo.Abp.Application.Services;
using Yi.Framework.Bbs.Application.Contracts.IServices;

namespace Yi.Framework.Bbs.Application.Services
{
    /// <summary>
    /// Setting服务实现
    /// </summary>
    public class SettingService : ApplicationService,
       ISettingService
    {

        /// <summary>
        /// 获取头像文件
        /// </summary>
        /// <returns></returns>
        public List<string> GetIcon()
        {
            return Directory.GetFiles("wwwroot/icon").Select(x => "wwwroot/icon/"+ Path.GetFileName(x)).ToList();
        }
    }
}
