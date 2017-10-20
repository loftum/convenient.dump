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
		private readonly Func<HttpContext, Func<HttpContext, Task<object>>, Task> _execute;

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
				_execute = Execute;
			}
			else
			{
				_execute = ExecuteWithViewEngine;
			}
		}

		public Task Invoke(HttpContext context)
		{
			var action = _controller.GetAction(context);
			return action == null ? _next.Invoke(context) : _execute(context, action);
		}

		private async Task Execute(HttpContext context, Func<HttpContext, Task<object>> action)
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
					context.Response.ContentType = "text/json";
					await context.Response.WriteAsync(_options.ToJson(result));
					break;
			}
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
							await _options.ViewEngine.WriteAsync(context, result);
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