using System.Security.Cryptography;
using Convenient.Dump.Core;
using ConvenientDump.LiteDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
	        app.UseFavicon();

	        app.Map("/db", a => a.UseDump(new DumpOptions
	        {
				DataStore = new LiteDbDataStore(),
				ToJson = JsonConvert.SerializeObject
	        }));

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Dump");
            });
        }
    }
}
