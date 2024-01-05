using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Yi.Framework.Bbs.Domain.Shared.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ArticleImportTypeEnum
    {
       [Description("默认导入方式")] 
        Default,

        [Description("vuePresss方式")] 
        VuePress
    }
}
