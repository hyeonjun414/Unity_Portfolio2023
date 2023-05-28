using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Util
{
    public static List<T> ToObjectList<T>(List<JObject> jsonList)
    {
        var objList = new List<T>();
        if (jsonList == null) return objList;
        
        foreach (var jObj in jsonList)
        {
            var obj = ToObject<T>(jObj);
            objList.Add(obj);
        }
        return objList;
    }
    public static T ToObject<T>(JObject data)
    {
        var type = typeof(T);
        var domainPrefix = type.FullName;
        domainPrefix = domainPrefix.Remove(domainPrefix.Length - type.Name.Length);
        if (data["Type"] != null)
        {
            type = Type.GetType(domainPrefix + data["Type"]);
        }

        return (T)JsonConvert.DeserializeObject(data.ToString(), type);
    }

    public static int CalcChance(double zeroToOne, List<float> tierChances)
    {
        var total = tierChances.Sum();
        double curr = 0;
        for (var index = 0; index < tierChances.Count; index++)
        {
            var f = tierChances[index];
            curr += f;
            if (zeroToOne < curr / total)
                return index;
        }

        return 0;
    }
    
}

public class EnumConverter<TEnum> : JsonConverter where TEnum : struct, Enum
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.String)
        {
            var stringValue = (string)reader.Value;
            if (Enum.TryParse<TEnum>(stringValue, out TEnum enumValue))
            {
                return enumValue;
            }
        }
        throw new JsonSerializationException($"Cannot convert \"{reader.Value}\" to {typeof(TEnum).Name}.");
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(TEnum);
    }
}