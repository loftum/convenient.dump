using System.IO;
using System.Text;

namespace Convenient.Dump.Core.App.Queries
{
	internal static class StreamExtensions
	{
		public static Stream ToStream(this string value)
		{
			var stream = new MemoryStream();
			var bytes = Encoding.UTF8.GetBytes(value);
			stream.Write(bytes, 0, bytes.Length);
			stream.Seek(0, SeekOrigin.Begin);
			return stream;
		}
	}
}