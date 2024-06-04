using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Abp.Tool.Domain.Shared.Options
{
    public class ToolOptions
    {
        /// <summary>
        /// 模块模板zip文件路径
        /// </summary>
        public string ModuleTemplateFilePath { get; set; }

        /// <summary>
        /// 项目模板zip文件路径
        /// </summary>
        public string ProjectTemplateFilePath { get; set; }


        /// <summary>
        /// 临时文件目录
        /// </summary>
        public string TempDirPath { get; set; }
    }
}
