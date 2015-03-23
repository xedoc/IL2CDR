using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace IL2CDR.Model
{
    public class Json
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            });
        }

        public static object Deserialize(string text)
        {
            return JsonConvert.DeserializeObject(text);
        }
    }
}
