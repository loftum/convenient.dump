using System.IO;
using System.Linq;
using Convenient.Dump.Core.App;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core
{
	internal static class HttpContextExtensions
	{
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
						       .FirstOrDefault() ?? ResponseTypes.Json;
			}
		}
	}
}