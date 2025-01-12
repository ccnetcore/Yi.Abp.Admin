using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yi.Framework.Core.Json;

public class DatetimeJsonConverter : JsonConverter<DateTime>
{
    private string _format;
    public DatetimeJsonConverter(string format="yyyy-MM-dd HH:mm:ss")
    {
        _format = format;
    }

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.TokenType==JsonTokenType.String)
        {
            if (DateTime.TryParse(reader.GetString(), out DateTime dateTime)) return dateTime;
        }
        return reader.GetDateTime();
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(_format));
    }
}