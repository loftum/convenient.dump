namespace ConvenientDump.LiteDb
{
	internal static class QueryHacks
	{
		private static readonly TypeCodeConverter Converter = new TypeCodeConverter();

		public static string Encode(this object value)
		{
			if (value == null)
			{
				return null;
			}
			var code = Converter[value.GetType()];
			return $"{value}:{code}";
		}

		public static object Decode(this string value)
		{
			if (value == null)
			{
				return null;
			}
			var parts = value.Split(':');
			if (parts.Length < 2 || !int.TryParse(parts[1], out var code))
			{
				return value;
			}
			return Converter[code](parts[0]);
		}
	}
}