using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Yi.Abp.Tool.Domain
{
    public class NugetCrawlerManager : ITransientDependency
    {
        private const string NugetVersionUrl = "https://www.nuget.org/packages/Yi.Abp.Tool#versions-body-tab";
        public NugetCrawlerManager(IDistributedCache<NugetResult> cache)
        {
            //缓存设置1分钟获取一次结果
            this.NugetResult = cache.GetOrAdd("NugetResult", () =>
              {
                  return InitData();
              }, () =>
              {
                  var options = new DistributedCacheEntryOptions();
                  options.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(1);
                  return options;
              })!;

        }
        private HtmlDocument HtmlDoc { get; set; }
        private NugetResult NugetResult { get; set; } = new NugetResult();

        private NugetResult InitData()
        {
            NugetResult nugetResult = new NugetResult();

            HtmlWeb web = new HtmlWeb();
            this.HtmlDoc = web.Load(NugetVersionUrl);
            nugetResult.Versions = GetVersionList();
            nugetResult.DownloadNumber = GetDownloadNumber();

            return nugetResult;
        }


        public NugetResult GetNugetResult()
        {
            return this.NugetResult;
        }

        /// <summary>
        /// 获取版本号列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetVersionList()
        {
            List<string> versions = new List<string>();

            var versionDoc = HtmlDoc.DocumentNode.SelectNodes("//*[@id=\"version-history\"]/table/tbody");
            var trDoc = versionDoc.First().ChildNodes.Where(x => x.Name == "tr").ToList();

            foreach (var tr in trDoc)
            {
                var version = tr.ChildNodes.Where(x => x.Name == "td").First().ChildNodes.Where(x => x.Name == "a").First().GetAttributes("title").First().Value;

                versions.Add(version);

            }
            return versions;
        }

        /// <summary>
        /// 获取下载总数
        /// </summary>
        /// <returns></returns>
        private long GetDownloadNumber()
        {
            var spanDoc = HtmlDoc.DocumentNode.SelectNodes("//*[@id=\"skippedToContent\"]/section/div/aside/div[1]/div[2]/div[1]/span[2]");
            var downLoadNumber = spanDoc.First().InnerText;
            return long.Parse(downLoadNumber);
        }
    }


    public class NugetResult
    {
        public long DownloadNumber { get; set; }
        public List<string> Versions { get; set; }
    }
}
