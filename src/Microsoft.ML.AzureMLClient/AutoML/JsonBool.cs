using System;
using Newtonsoft.Json;

public class FirstCapBooleanJsonConverter : JsonConverter
{
    public override bool CanRead { get { return false; } }
    public override bool CanWrite { get { return true; } }

    public override bool CanConvert(Type objectType)
    {
        // Handle only boolean types.
        return objectType == typeof(bool);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteRawValue((bool)value ? "True" : "False");
    }
}
