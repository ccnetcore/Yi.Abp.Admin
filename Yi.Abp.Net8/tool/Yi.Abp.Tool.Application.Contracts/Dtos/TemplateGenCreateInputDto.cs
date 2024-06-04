using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yi.Abp.Tool.Domain.Shared.Enums;

namespace Yi.Abp.Tool.Application.Contracts.Dtos
{
    public class TemplateGenCreateInputDto
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 数据库提供者
        /// </summary>
        public DbmsEnum Dbms { get; set; }


        /// <summary>
        /// 需要替换的字符串内容
        /// </summary>
        public Dictionary<string, string> ReplaceStrData { get; set; }=new Dictionary<string, string>();

        public void SetNameReplace()
        {
            ReplaceStrData.Add("Yi.Abp", Name);
            ReplaceStrData.Add("YiAbp", Name.Replace(".", ""));
        }
    }
}
