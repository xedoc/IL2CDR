using Newtonsoft.Json;

namespace IL2CDR.Model
{
	public class Json
	{
		public static string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings() {
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
			});
		}

		public static string Serialize(object obj, JsonConverter converter)
		{
			JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
			};

			return JsonConvert.SerializeObject(obj, Formatting.None, converter);
		}

		public static object Deserialize(string text)
		{
			return JsonConvert.DeserializeObject(text);
		}
	}
}