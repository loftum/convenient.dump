using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Dump.Core.App.IO;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	internal class ViewRenderer
	{
		private readonly FileLoader _fileLoader = new FileLoader();

		public async Task<string> Render(HttpContext context, string view, object model)
		{
			var html = await _fileLoader.GetAsync($"Views/{view}.html");
			var dictionary = new Dictionary<string, object>
			{
				["@BasePath"] = context.Request.PathBase
			};
			
			return DoRender(html, dictionary);
		}

		private string DoRender(string template, Dictionary<string, object> dictionary)
		{
			return dictionary.Aggregate(template, (t, pair) => t.Replace(pair.Key, pair.Value.ToString()));
		}
	}
}