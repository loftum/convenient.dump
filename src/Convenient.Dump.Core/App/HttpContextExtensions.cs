﻿using Convenient.Dump.Core.App.Queries;
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
	}
}