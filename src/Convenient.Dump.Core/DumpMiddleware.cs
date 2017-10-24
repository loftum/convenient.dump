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

		public async Task Invoke(HttpContext context)
		{
			try
			{
				var action = _controller.GetAction(context);
				if (action == null)
				{
					await _next.Invoke(context);
				}
				else
				{
					await Execute(context, action).ConfigureAwait(false);
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

	internal class ExceptionModel
	{
		public string Type { get; set; }
		public string Message { get; set; }
		public string StackTrace { get; set; }
		public ExceptionModel InnerException { get; set; }

		public ExceptionModel(Exception ex)
		{
			Type = ex?.GetType().Name;
			Message = ex?.Message ?? "There is no spoon";
			StackTrace = ex?.StackTrace;
			InnerException = ex.InnerException == null ? null : new ExceptionModel(ex.InnerException);
		}
	}
}