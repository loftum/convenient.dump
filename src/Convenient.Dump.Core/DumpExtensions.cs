using Microsoft.AspNetCore.Builder;

namespace Convenient.Dump.Core
{
	public static class DumpExtensions
	{
		public static IApplicationBuilder UseDump(this IApplicationBuilder app, DumpOptions options)
		{
			return app.UseMiddleware<DumpMiddleware>(options);
		}
	}
}