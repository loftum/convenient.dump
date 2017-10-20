using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Convenient.Dump.Core.App.IO;
using Microsoft.AspNetCore.Http;

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
			Route("GET", "^/Content/angular-js$", GetAngular);
			Route("GET", "^/Content/site-css$", GetSiteCss);
			Route("GET", "^/?$", Index);
			Route("GET", "^/(?<collection>[a-zA-Z_]+){1}(\\.[a-zA-Z]+)?/?$", QueryCollection);
			Route("GET", "^/(?<collection>[a-zA-Z_]+)/(?<id>[a-zA-Z0-9]+){1}/?$", GetItem);
			Route("POST", "^/(?<collection>[a-zA-Z_]+){1}$", SaveItem);
			Route("DELETE", "^/(?<collection>[a-zA-Z_]+)/(?<id>[a-zA-Z0-9]+){1}/?$", RemoveItem);
			Route("DELETE", "^/(?<collection>[a-zA-Z_]+){1}/?$", DropCollection);
		}

		private async Task<object> GetSiteCss(HttpContext context, Match match)
		{
			var file = await _fileLoader.GetAsync("Content/site.css").ConfigureAwait(false);
			return new Response
			{
				Handle = r =>
				{
					r.ContentType = "text/css";
					return r.WriteAsync(file);
				}
			};
		}

		private async Task<object> GetAngular(HttpContext context, Match match)
		{
			var file = await _fileLoader.GetAsync("Content/angular.min.js").ConfigureAwait(false);
			return new Response
			{
				Handle = r =>
				{
					r.ContentType = "text/javascript";
					return r.WriteAsync(file);
				}
			};
		}

		private async Task<object> Index(HttpContext context, Match match)
		{
			var db = await _options.DataStore.GetInfo().ConfigureAwait(false);
			return db;
		}

		private async Task<object> GetItem(HttpContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			var id = match.Groups["id"].Value;
			var result = await _options.DataStore.Get(collection, id).ConfigureAwait(false);
			return result;
		}

		private async Task<object> RemoveItem(HttpContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			var id = match.Groups["id"].Value;
			var result = await _options.DataStore.RemoveItem(collection, id).ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = result ? $"Deleted {collection}/{id}" : $"Did not delete {collection}/{id}"
			};
		}

		private async Task<object> DropCollection(HttpContext context, Match match)
		{
			var collection = match.Groups["collection"].Value;
			await _options.DataStore.DropCollection(collection).ConfigureAwait(false);
			return new ResponseMessage
			{
				Message = $"Dropped {collection}"
			};
		}

		private async Task<object> SaveItem(HttpContext context, Match match)
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

		private void Route(string method, string pathPattern, Func<HttpContext, Match, Task<object>> action)
		{
			_routes.Add(new SimpleRoute(method, pathPattern, action));
		}

		private async Task<object> QueryCollection(HttpContext context, Match match)
		{
			var parameters = context.GetQueryParameters();
			var result = await _options.DataStore.QueryCollection(match.Groups["collection"].Value, parameters).ConfigureAwait(false);
			return result;
		}

		public Func<HttpContext, Task<object>> GetAction(HttpContext context)
		{
			return _routes.Select(r => r.GetAction(context)).FirstOrDefault(a => a != null);
		}
	}
}