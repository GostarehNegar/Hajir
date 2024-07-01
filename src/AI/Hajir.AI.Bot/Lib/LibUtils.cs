using System;
using System.Collections.Generic;
using System.Text;

namespace GostarehNegarBot.Lib
{
    public class LibUtils
    {
        public static string Serialize(object data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
        public static T Deserialize<T>(string data) => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(data);
        public static byte[] Encode(object data) => System.Text.Encoding.UTF8.GetBytes(Serialize(data));

        public static string Decode(byte[] data) => System.Text.Encoding.UTF8.GetString(data);
    }
}
