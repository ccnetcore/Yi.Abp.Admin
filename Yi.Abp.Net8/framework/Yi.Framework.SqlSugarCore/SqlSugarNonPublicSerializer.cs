using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;

namespace Yi.Framework.SqlSugarCore;

public class NonPublicPropertiesResolver : DefaultContractResolver
{
    /// <summary>
    /// 重写获取属性，存在get set方法就可以写入
    /// </summary>
    /// <param name="member"></param>
    /// <param name="memberSerialization"></param>
    /// <returns></returns>
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);
        if (member is PropertyInfo pi)
        {
            prop.Readable = (pi.GetMethod != null);
            prop.Writable = (pi.SetMethod != null);
        }

        return prop;
    }
}

public class SqlSugarNonPublicSerializer : ISerializeService
{
    /// <summary>
    /// 默认的序列化服务
    /// </summary>
    private readonly ISerializeService _serializeService = DefaultServices.Serialize;

    public string SerializeObject(object value)
    {
        //保留原有实现
        return  _serializeService.SerializeObject(value);
    }

    public string SugarSerializeObject(object value)
    { //保留原有实现
        return  _serializeService.SugarSerializeObject(value);
    }
    
    /// <summary>
    /// 重写对象反序列化支持NoPublic访问器
    /// </summary>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T DeserializeObject<T>(string value)
    {
        if (typeof(T).FullName.StartsWith("System.Text.Json."))
        {
            // 动态创建一个 JsonSerializer 实例
            Type serializerType =typeof(T).Assembly.GetType("System.Text.Json.JsonSerializer");

            var methods = serializerType
                .GetMethods().Where(it=>it.Name== "Deserialize")
                .Where(it=>it.GetParameters().Any(z=>z.ParameterType==typeof(string))).First();

            // 调用 SerializeObject 方法序列化对象
            T json = (T)methods.MakeGenericMethod(typeof(T))
                .Invoke(null, new object[] { value, null! });
            return json!;
        }
        var jSetting = new JsonSerializerSettings 
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver =new NonPublicPropertiesResolver() //替换默认解析器使能支持protect
        };
        return JsonConvert.DeserializeObject<T>(value, jSetting)!;
    }
}