using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Ses.AspNetCore.Framework.Helper
{
    public static class JsonHelp
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(this string json)
        {
            return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
        }

        public static List<T> DeserializeList<T>(this string json)
        {
            return string.IsNullOrEmpty(json) ? default(List<T>) : JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}
