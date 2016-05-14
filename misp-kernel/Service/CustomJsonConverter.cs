using Misp.Kernel.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class CustomJsonConverter : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransformationTreeItem);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            try
            {
                JToken token = null;
                JObject jsonObject = JObject.Load(reader);                
                if (jsonObject.TryGetValue("typeName", out token))
                {
                    switch ((string)token)
                    {
                        case "TransformationTreeLoop": {
                            TransformationTreeItem loop = jsonObject.ToObject<TransformationTreeItem>(serializer);
                            return loop; 
                        }
                        case "TransformationTreeAction": {
                            TransformationTreeItem action = jsonObject.ToObject<TransformationTreeItem>(serializer);
                            return action;
                        }
                        default: {
                            TransformationTreeItem action = jsonObject.ToObject<TransformationTreeItem>(serializer);
                            return action;
                        }
                    }
                }
                return jsonObject.ToObject<TransformationTreeItem>(serializer);
            }
            catch (Exception)
            {
                return null;
            } 
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}