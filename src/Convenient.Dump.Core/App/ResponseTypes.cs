using System.Linq;
using System.Reflection;

namespace Convenient.Dump.Core.App
{
	internal static class ResponseTypes
	{
		public const string Html = "html";
		public const string Json = "json";

		public static readonly string[] All;

		static ResponseTypes()
		{
			All = typeof(ResponseTypes)
				.GetFields(BindingFlags.Public | BindingFlags.Static)
				.Where(f => f.FieldType == typeof(string))
				.Select(f => f.GetValue(null))
				.Cast<string>()
				.ToArray();
		}
	}
}