using System.Reflection;
using System.Web;
using Yi.Framework.WeChat.MiniProgram.Abstract;

namespace Yi.Framework.WeChat.MiniProgram;

public static class WeChatMiniProgramExtensions
{
    /// <summary>
    /// 效验请求是否成功
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    internal static void ValidateSuccess(this IErrorObjct response)
    {

        if (response.errcode != 0)
        {
            throw new WeChatMiniProgramException(response.errmsg);
        }
    }

    internal static string ToQueryString<T>(this T obj)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var queryParams = new List<string>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj, null);
            if (value != null)
            {
                // 处理集合
                if (value is IEnumerable<object> enumerable)
                {
                    foreach (var item in enumerable)
                    {
                        queryParams.Add($"{HttpUtility.UrlEncode(prop.Name)}={HttpUtility.UrlEncode(item.ToString())}");
                    }
                }
                else
                {
                    queryParams.Add($"{HttpUtility.UrlEncode(prop.Name)}={HttpUtility.UrlEncode(value.ToString())}");
                }
            }
        }

        return string.Join("&", queryParams);
    }
}