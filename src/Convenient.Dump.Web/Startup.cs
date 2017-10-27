using System.Threading.Tasks;
using Convenient.Dump.Core;
using ConvenientDump.LiteDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Convenient.Dump.Web
{
	public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
			
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
			if (Config.Current.RequireHttps)
			{
				app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
			}
			app.UseFavicon();

	        app.Map("/db", a => a.UseDump(new DumpOptions
	        {
				DataStore = new LiteDbDataStore(),
				ToJson = JsonConvert.SerializeObject
	        }));

            app.Run(context =>
            {
				context.Response.Redirect("/db");
				return Task.CompletedTask;
            });
        }
    }
}
