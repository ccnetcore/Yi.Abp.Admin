using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using Yi.Framework.Core.Helper;
using Yi.Framework.Rbac.Application.Contracts.IServices;

namespace Yi.Framework.Rbac.Application.Services.Monitor
{
    public class MonitorServerService : ApplicationService, IMonitorServerService
    {
        private IWebHostEnvironment _hostEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        public MonitorServerService(IWebHostEnvironment hostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet("monitor-server/info")]
        public object GetInfo()
        {
            int cpuNum = Environment.ProcessorCount;
            string computerName = Environment.MachineName;
            string osName = RuntimeInformation.OSDescription;
            string osArch = RuntimeInformation.OSArchitecture.ToString();
            string version = RuntimeInformation.FrameworkDescription;
            string appRAM = ((double)Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB";
            string startTime = Process.GetCurrentProcess().StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string sysRunTime = ComputerHelper.GetRunTime();
            string serverIP = _httpContextAccessor.HttpContext.Connection.LocalIpAddress.MapToIPv4().ToString() + ":" + _httpContextAccessor.HttpContext.Connection.LocalPort;//获取服务器IP

            var programStartTime = Process.GetCurrentProcess().StartTime;
            string programRunTime = DateTimeHelper.FormatTime(long.Parse((DateTime.Now - programStartTime).TotalMilliseconds.ToString().Split('.')[0]));
            var data = new
            {
                cpu = ComputerHelper.GetComputerInfo(),
                disk = ComputerHelper.GetDiskInfos(),
                sys = new { cpuNum, computerName, osName, osArch, serverIP, runTime = sysRunTime },
                app = new
                {
                    name = _hostEnvironment.EnvironmentName,
                    rootPath = _hostEnvironment.ContentRootPath,
                    webRootPath = _hostEnvironment.WebRootPath,
                    version,
                    appRAM,
                    startTime,
                    runTime = programRunTime,
                    host = serverIP
                },
            };

            return data;
        }

    }
}
