using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Convenient.Dump.Testing
{
	public static class Json
	{
		private static readonly JsonSerializerSettings Settings;

		static Json()
		{
			var settings = new JsonSerializerSettings();
			settings.Converters.Add(new StringEnumConverter());
			Settings = settings;
		}

		public static string Serialize(object o, bool indented = false)
		{
			var formatting = indented ? Formatting.Indented : Formatting.None;
			return JsonConvert.SerializeObject(o, formatting, Settings);
		}
	}
}