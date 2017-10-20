using System;
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

		public Task Invoke(HttpContext context)
		{
			var action = _controller.GetAction(context);
			return action == null ? _next.Invoke(context) : ExecuteWithViewEngine(context, action);
		}

		private async Task ExecuteWithViewEngine(HttpContext context, Func<HttpContext, Task<object>> action)
		{
			var result = await action(context).ConfigureAwait(false);
			switch (result)
			{
				case Response r:
					await r.Handle(context.Response).ConfigureAwait(false);
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