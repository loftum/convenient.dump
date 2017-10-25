using System;
using System.IO;
using System.Threading.Tasks;

namespace Convenient.Dump.Core.App
{
	public class SimpleResponse
	{
		public string ContentType { get; set; }
		public Func<Stream, Task> WriteAsync { get; set; }

		public SimpleResponse()
		{
			WriteAsync = s => Task.CompletedTask;
		}
	}
}