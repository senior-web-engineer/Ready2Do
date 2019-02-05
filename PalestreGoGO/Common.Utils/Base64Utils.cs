using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.Utils
{
    public static class Base64Utils
    {
        public static string ToBase64Binary(this object obj)
        {
            byte[] buffer;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                buffer = ms.ToArray();
            }
            return Convert.ToBase64String(buffer);
        }

        public static T FromBase64Binary<T>(this string str) where T : class
        {
            T result = default(T);
            using (var ms = new MemoryStream(Convert.FromBase64String(str)))
            {
                BinaryFormatter bf = new BinaryFormatter();
                result = bf.Deserialize(ms) as T;
            }
            return result;
        }

        public static string ToBase64Json(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public static T FromBase64Json<T>(this string value)
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
