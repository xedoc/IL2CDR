using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace IL2CDR.Model
{
    public class Json
    {
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static string Serialize(object obj)
        {
            return serializer.Serialize(obj);
        }

        public static object Deserialize(string text)
        {
            return serializer.DeserializeObject(text);
        }
    }
}
