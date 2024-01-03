using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Yi.Abp.Application.Services
{
    public class TestService : ApplicationService
    {
        /// <summary>
        /// 你好世界
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetHelloWorld(string? name)
        {
            return name ?? "HelloWord";
        }
    }
}
