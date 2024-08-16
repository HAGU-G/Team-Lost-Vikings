using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntConverter : JsonConverter<Dictionary<Vector2Int, int>>
{
    public override Dictionary<Vector2Int, int> ReadJson(JsonReader reader, Type objectType, Dictionary<Vector2Int, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var result = new Dictionary<Vector2Int, int>();

        foreach (var property in jObject.Properties())
        {
            string[] coordinates = property.Name.Trim('(', ')').Split(',');
            Vector2Int key = new Vector2Int(int.Parse(coordinates[0]), int.Parse(coordinates[1]));
            int value = property.Value.ToObject<int>();
            result.Add(key, value);
        }

        return result;
    }

    public override void WriteJson(JsonWriter writer, Dictionary<Vector2Int, int> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            string key = $"({kvp.Key.x},{kvp.Key.y})";
            writer.WritePropertyName(key);
            writer.WriteValue(kvp.Value);
        }
        writer.WriteEndObject();
    }
}
