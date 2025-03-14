using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yi.Framework.SqlSugarCore.Abstractions
{
    /// <summary>
    /// 忽略CodeFirst特性
    /// 标记此特性的实体类将不会被CodeFirst功能扫描
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreCodeFirstAttribute : Attribute
    {
    }
}
