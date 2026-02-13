using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmallTask.Json;

/// <summary>
/// Serializes DateTime as UTC (with Z suffix) when Kind is Unspecified.
/// EF returns DateTime from SQL Server as Unspecified; we store UTC, so treat as UTC for correct client display.
/// </summary>
public class UtcDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetDateTime();

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utc = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value.ToUniversalTime();
        writer.WriteStringValue(utc);
    }
}
