using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Yi.Abp.Tool
{
    public interface ICommand:ITransientDependency
    {
        /// <summary>
        /// 命令串
        /// </summary>
        public List<string> CommandStrs { get; }

        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        public Task InvokerAsync(Dictionary<string,string> options, string[] args);
    }
}
