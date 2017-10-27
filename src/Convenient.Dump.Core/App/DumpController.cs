using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Convenient.Dump.Core.App.IO;

namespace Convenient.Dump.Core.App
{
	internal class DumpController
	{
		private readonly DumpOptions _options;
		private readonly List<SimpleRoute> _routes = new List<SimpleRoute>();
		private readonly FileLoader _fileLoader = new FileLoader();

		public DumpController(DumpOptions options)
		{
			_options = options;
			Route("GET", "^/?$", DbInfo);
			Route("GET", "^/Content/(?<filename>[a-zA-Z_-]+){1}/?$", GetStaticFile);
			
			Route("GET", "^/(?<collection>[a-zA-Z_]+){1}(\\.[a-zA-Z]+)?/?$", QueryCollection);

			Route("GET", "^/(?<collection>[a-zA-Z_]+)/(?<id>[a-zA-Z0-9-_]+){1}(\\.[a-zA-Z]+)?/?$", GetItem);
			Route("DELETE", "^/(?<collection>[a-zA-Z_]+)/(?<id>[a-zA-Z0-9]+){1}(\\.[a-zA-Z]+)?/?$", RemoveItem);

			Route("POST", "^/(?<collection>[a-zA-Z_]+){1}/?$", SaveItem);
			
			Route("DELETE", "^/(?<collection>[a-zA-Z_]+){1}/\\*?$", ClearCollection);
			Route("DELETE", "^/\\*/\\*?$", ClearAllCollections);
			Route("DELETE", "^/(?<collection>[a-zA-Z_]+){1}/?$", DropCollection);
		}

		private async Task<object> GetStaticFile(SimpleContext context, Match match)
		{
			var filename = match.Groups["filename"].Value.Replace("-", ".");
			var file = await _fileLoader.GetAsync($"Content/{filename}").ConfigureAwait(false);

			return new SimpleResponse
			{
				ContentType = GetContentType(filename),
				WriteAsync = r => r.WriteAsync(file)
			};
		}

		private static string GetContentType(string filename)
		{
			var extension = Path.GetExtension(filename).Trim('.');
			switch (extension)
			{
				case "js": return "text/javascript";
				case "css": return "text/css";
				case "html": return "text/html";
				default: return "text/plain";
			}
		}

		private async Task<object> DbInfo(SimpleContext context, Match match)
		{
			context.ViewBag["Title"] = "Index";
			var db = await _options.DataStore.GetInfo().ConfigureAwait(false);
			return db;
		}

		private async Task<object> GetItem(SimpleContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			
			var id = match.Groups["id"].Value;
			context.ViewBag["Title"] = id;
			var result = await _options.DataStore.Get(collection, id).ConfigureAwait(false);
			return result;
		}

		private async Task<object> RemoveItem(SimpleContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			var id = match.Groups["id"].Value;
			var result = await _options.DataStore.RemoveItem(collection, id).ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = result ? $"Deleted {collection}/{id}" : $"Did not delete {collection}/{id}"
			};
		}

		private async Task<object> ClearCollection(SimpleContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			await _options.DataStore.ClearCollection(collection).ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = $"Cleared {collection}"
			};
		}

		private async Task<object> ClearAllCollections(SimpleContext context, Match match)
		{
			await _options.DataStore.ClearAllCollections().ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = "Cleared all collections"
			};
		}

		private async Task<object> DropCollection(SimpleContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			await _options.DataStore.DropCollection(collection).ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = $"Dropped {collection}"
			};
		}

		private async Task<object> SaveItem(SimpleContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			using (var reader = new StreamReader(context.Request.Body))
			{
				var json = await reader.ReadToEndAsync().ConfigureAwait(false);
				await _options.DataStore.Save(collection, json).ConfigureAwait(false);
			}
			return new ResponseMessage
			{
				Message = $"Saved to {collection}"
			};
		}

		private void Route(string method, string pathPattern, Func<SimpleContext, Match, Task<object>> action)
		{
			_routes.Add(new SimpleRoute(method, pathPattern, action));
		}

		private async Task<object> QueryCollection(SimpleContext context, Match match)
		{
			var parameters = context.GetQueryParameters();
			var collection = match.Groups["collection"].Value;
			context.ViewBag["Title"] = collection;
			var result = await _options.DataStore.QueryCollection(collection, parameters).ConfigureAwait(false);
			return result;
		}

		public Func<SimpleContext, Task<object>> GetAction(SimpleContext context)
		{
			return _routes.Select(r => r.GetAction(context)).FirstOrDefault(a => a != null);
		}
	}
}