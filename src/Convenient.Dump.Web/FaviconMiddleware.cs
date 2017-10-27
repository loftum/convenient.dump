using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Web
{
	public class FaviconOptions
	{
		public byte[] Favicon { get; set; }
	}

	public class FaviconMiddleware
	{
		private readonly FaviconOptions _options;
		private readonly RequestDelegate _next;

		public FaviconMiddleware(RequestDelegate next, FaviconOptions options)
		{
			_next = next;
			_options = options;
		}

		public Task Invoke(HttpContext context)
		{
			if (context.Request.Path != new PathString("/favicon.ico"))
			{
				return _next.Invoke(context);
			}
			context.Response.ContentType = "image/vnd.microsoft.icon";
			return context.Response.Body.WriteAsync(_options.Favicon, 0, _options.Favicon.Length);
		}
	}

	public static class FaviconExtensions
	{
		public static IApplicationBuilder UseFavicon(this IApplicationBuilder app, string filename = null)
		{
			return app.UseMiddleware<FaviconMiddleware>(new FaviconOptions
			{
				Favicon = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename ?? "favicon.ico"))
			});
		}
	}
}