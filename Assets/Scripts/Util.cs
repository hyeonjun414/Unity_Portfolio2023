using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Util
{
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
}