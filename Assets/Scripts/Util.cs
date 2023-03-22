using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Util
{
    public static List<T> ToObjectList<T>(List<JObject> jsonList)
    {
        var objList = new List<T>();
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
}