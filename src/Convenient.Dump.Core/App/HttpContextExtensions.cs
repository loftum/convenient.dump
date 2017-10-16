using Convenient.Dump.Core.Data;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	internal static class HttpContextExtensions
	{
		public static QueryParameters GetQueryParameters(this HttpContext context)
		{
			var query = context.Request.Query;
			var parameters = new QueryParameters();
			foreach (var key in query.Keys)
			{
				int number;
				switch (key.ToLowerInvariant())
				{
					case "skip":
						parameters.Skip = int.TryParse(query[key], out number) ? number : 0;
						break;
					case "take":
						parameters.Take = int.TryParse(query[key], out number) ? number : 0;
						break;
					default:
						parameters.Fields[key] = query[key];
						break;
				}
			}
			return new QueryParameters();
		}
	}
}