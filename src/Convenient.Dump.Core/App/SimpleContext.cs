using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Convenient.Dump.Core.App
{
	public class SimpleContext
	{
		public SimpleRequest Request { get; set; } = new SimpleRequest();
	}

	public class SimpleRequest
	{
		public string Method { get; set; }
		public string Path { get; set; }
		public SimpleHeaderDictionary Headers { get; set; }
		public Dictionary<string, string[]> Query { get; set; }
		public Stream Body { get; set; }
	}

	public class SimpleHeaderDictionary
	{
		private readonly IDictionary<string, string> _headers;

		public SimpleHeaderDictionary(IEnumerable<KeyValuePair<string, string>> headers)
		{
			_headers = headers.ToDictionary(h => h.Key, h => h.Value);
		}

		public string this[string key] => _headers.TryGetValue(key, out var value) ? value : null;
	}
}