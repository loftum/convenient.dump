using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Dump.Core.App.IO;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	internal class ViewEngine : IViewEngine
	{
		private readonly FileLoader _fileLoader = new FileLoader();
		private readonly Func<object, string> _toJson;

		public ViewEngine(Func<object, string> toJson)
		{
			_toJson = toJson;
		}

		public async Task<string> Render(HttpContext context, object model)
		{
			if (model == null)
			{
				return "<html><head><meta charset=\"UTF-8\"/><title>Null</title></head><body>Null</body></html>";
			}
			var html = await _fileLoader.GetAsync($"Views/{model.GetType().Name}.html");
			var dictionary = new Dictionary<string, object>
			{
				["@BasePath"] = context.Request.PathBase,
				["@Path"] = context.Request.Path,
				["@Model"] = _toJson(model)
			};
			
			return DoRender(html, dictionary);
		}

		private static string DoRender(string template, Dictionary<string, object> dictionary)
		{
			return dictionary.Aggregate(template, (t, pair) => t.Replace(pair.Key, pair.Value.ToString()));
		}
	}
}