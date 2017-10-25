using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Convenient.Dump.Core.App
{
	internal static class StreamExtensions
	{
		public static Task WriteAsync(this Stream stream, string text, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (text == null)
				throw new ArgumentNullException(nameof(text));
			return stream.WriteAsync(text, Encoding.UTF8, cancellationToken);
		}

		public static Task WriteAsync(this Stream stream, string text, Encoding encoding, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (text == null)
				throw new ArgumentNullException(nameof(text));
			if (encoding == null)
				throw new ArgumentNullException(nameof(encoding));
			var bytes = encoding.GetBytes(text);
			return stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
		}
	}
}