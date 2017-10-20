using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Convenient.Dump.Core.App
{
	public class Response
	{
		public Func<HttpResponse, Task> Handle { get; set; }

		public Response()
		{
			Handle = r => Task.CompletedTask;
		}
	}
}