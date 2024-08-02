using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Yi.Abp.Tool.Domain;

namespace Yi.Abp.Tool.Application
{
    public class NueGetInfoService : ApplicationService
    {
        private NugetCrawlerManager _nugetCrawlerManager;
        public NueGetInfoService(NugetCrawlerManager nugetCrawlerManager) { _nugetCrawlerManager = nugetCrawlerManager; }

        /// <summary>
        /// 获取爬虫结果
        /// </summary>
        /// <returns></returns>
        public NugetResult GetInfo()
        {
            return _nugetCrawlerManager.GetNugetResult();
        }
    }
}
