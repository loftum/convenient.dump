using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Dump.Core.App;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core
{
	public class DumpMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly DumpOptions _options;
		private readonly DumpController _controller;

		public DumpMiddleware(RequestDelegate next, DumpOptions options)
		{
			if (options.ToJson == null)
			{
				throw new ArgumentNullException(nameof(options.ToJson));
			}
			if (options.DataStore == null)
			{
				throw new ArgumentNullException(nameof(options.DataStore));
			}
			_next = next;
			_options = options;
			_controller = new DumpController(options);
			if (options.ViewEngine == null)
			{
				options.ViewEngine = new ViewEngine(options.ToJson);
			}
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				var simple = new SimpleContext
				{
					Request = new SimpleRequest
					{
						Method = context.Request.Method,
						Path = context.Request.Path,
						Headers = new SimpleHeaderDictionary(context.Request.Headers.Select(h => new KeyValuePair<string, string>(h.Key, h.Value))),
						Query = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToArray()),
						Body = context.Request.Body
					}
				};
				var action = _controller.GetAction(simple);
				if (action == null)
				{
					await _next.Invoke(context);
				}
				else
				{
					await Execute(context, simple, action).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				var model = new ExceptionModel(ex);
				context.Response.StatusCode = 500;
				switch (context.GetResponseType())
				{
					case ResponseTypes.Html:
						context.Response.ContentType = "text/html";
						var html = await _options.ViewEngine.Render(context, model);
						await context.Response.WriteAsync(html).ConfigureAwait(false);
						break;
					default:
						context.Response.ContentType = "text/json";
						await context.Response.WriteAsync(_options.ToJson(model));
						break;
				}
			}
		}

		private async Task Execute(HttpContext context, SimpleContext simple, Func<SimpleContext, Task<object>> action)
		{
			var result = await action(simple).ConfigureAwait(false);
			switch (result)
			{
				case SimpleResponse r:
					await r.WriteAsync(context.Response.Body).ConfigureAwait(false);
					break;
				case null:
					context.Response.ContentType = "text/plain";
					await context.Response.WriteAsync("null").ConfigureAwait(false);
					break;
				default:
					switch (context.GetResponseType())
					{
						case ResponseTypes.Html:
							context.Response.ContentType = "text/html";
							var html = await _options.ViewEngine.Render(context, result);
							await context.Response.WriteAsync(html).ConfigureAwait(false);
							break;
						default:
							context.Response.ContentType = "text/json";
							await context.Response.WriteAsync(_options.ToJson(result));
							break;
					}
					break;
			}
		}
	}
}