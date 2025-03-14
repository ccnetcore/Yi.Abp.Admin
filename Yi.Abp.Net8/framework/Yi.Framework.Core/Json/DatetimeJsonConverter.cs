using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yi.Framework.Core.Json;

/// <summary>
/// DateTime JSON序列化转换器
/// </summary>
public class DatetimeJsonConverter : JsonConverter<DateTime>
{
    private readonly string _dateFormat;

    /// <summary>
    /// 初始化DateTime转换器
    /// </summary>
    /// <param name="format">日期格式化字符串,默认为yyyy-MM-dd HH:mm:ss</param>
    public DatetimeJsonConverter(string format = "yyyy-MM-dd HH:mm:ss")
    {
        _dateFormat = format;
    }

    /// <summary>
    /// 从JSON读取DateTime值
    /// </summary>
    /// <param name="reader">JSON读取器</param>
    /// <param name="typeToConvert">目标类型</param>
    /// <param name="options">JSON序列化选项</param>
    /// <returns>DateTime值</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return DateTime.TryParse(reader.GetString(), out DateTime dateTime) 
                ? dateTime 
                : reader.GetDateTime();
        }
        return reader.GetDateTime();
    }

    /// <summary>
    /// 将DateTime写入JSON
    /// </summary>
    /// <param name="writer">JSON写入器</param>
    /// <param name="value">DateTime值</param>
    /// <param name="options">JSON序列化选项</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_dateFormat));
    }
}