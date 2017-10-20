using System.IO;
using System.Linq;
using Convenient.Dump.Core.App.Queries;
using Convenient.Dump.Core.Data;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	internal static class HttpContextExtensions
	{
		public static QueryInput GetQueryParameters(this HttpContext context)
		{
			var query = context.Request.Query;
			var parameters = new QueryInput();
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
					case "q":
						parameters.Query = QueryParser.Parse(query[key]);
						break;
				}
			}

			return parameters;
		}

		public static string GetResponseType(this HttpContext context)
		{
			var extension = Path.GetExtension(context.Request.Path)?.ToLowerInvariant().Trim('.');
			switch (extension)
			{
				case "json": return ResponseTypes.Json;
				case "html": return ResponseTypes.Html;
				default:
					var acceptHeader = context.Request.Headers["Accept"];
					return acceptHeader.SelectMany(h => ResponseTypes.All.Where(h.Contains))
						       .FirstOrDefault() ?? ResponseTypes.Html;
			}
		}
	}
}