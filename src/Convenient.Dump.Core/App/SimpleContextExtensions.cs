using System.Linq;
using Convenient.Dump.Core.App.Queries;
using Convenient.Dump.Core.Data;

namespace Convenient.Dump.Core.App
{
	internal static class SimpleContextExtensions
	{
		public static QueryInput GetQueryParameters(this SimpleContext context)
		{
			var query = context.Request.Query;
			var parameters = new QueryInput();
			foreach (var key in query.Keys)
			{
				int number;
				switch (key.ToLowerInvariant())
				{
					case "skip":
						parameters.Skip = int.TryParse(query[key].LastOrDefault(), out number) ? number : 0;
						break;
					case "take":
						parameters.Take = int.TryParse(query[key].LastOrDefault(), out number) ? number : 0;
						break;
					case "q":
						parameters.Query = QueryParser.Parse(query[key].LastOrDefault());
						break;
				}
			}
			return parameters;
		}
	}
}