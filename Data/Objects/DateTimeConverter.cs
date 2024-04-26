using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ALViN.Data.Utility;
/// <summary>
/// To fix a strange formatting difference on Pocketbases Side. 
/// </summary>
public class DateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeStr = reader.GetString();
        var format = "yyyy-MM-dd HH:mm:ss.fff'Z'";
        if(DateTime.TryParseExact(dateTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date))
        {
            return date;
        }
        else
        {
            throw new JsonException("DateTime was not in the expected format.");
        }
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
           writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture));
    }

}