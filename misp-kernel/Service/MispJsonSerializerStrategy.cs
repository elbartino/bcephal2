using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misp.Kernel.Service
{
    public class MispJsonSerializerStrategy : RestSharp.PocoJsonSerializerStrategy
    {

        public override object DeserializeObject(object value, Type type)
        {
            
            try
            {
                if (type == typeof(Domain.Target) && value != null)
                {
                    RestSharp.JsonObject jsonObject = (RestSharp.JsonObject)value;
                    object typeName = null;
                    if (jsonObject.TryGetValue("typeName", out typeName))
                    {
                        switch ((string)typeName)
                        {
                            case "Model": { type = typeof(Domain.Model); break; }
                            case "Entity": { type = typeof(Domain.Entity); break; }
                            case "Attribute": { type = typeof(Domain.Attribute); break; }
                            case "AttributeValue": { type = typeof(Domain.AttributeValue); break; }
                            case "Target": { type = typeof(Domain.Target); break; }
                            default: { type = typeof(Domain.Target); break; }
                        }
                    }
                }
                
                object obj = base.DeserializeObject(value, type);
                return obj;
            }
            catch (Exception)
            {
                return null;
            } 
        }


    }
}
