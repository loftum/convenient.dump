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
		}

		public Task Invoke(HttpContext context)
		{
			var action = _controller.GetAction(context);
			return action == null ? _next.Invoke(context) : Execute(context, action);
		}

		private async Task Execute(HttpContext context, Func<HttpContext, Task<object>> action)
		{
			var result = await action(context);
			context.Response.ContentType = "text/json";
			await context.Response.WriteAsync(_options.ToJson(result));
		}
	}
}