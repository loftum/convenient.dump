using System.Collections.Generic;

namespace Convenient.Dump.Core.Data
{
	public class QueryInput
	{
		public int Skip { get; set; }
		public int Take { get; set; } = 20;

		public Dictionary<string, string> Parameters { get; } = new Dictionary<string, string>();

		public Dictionary<string, object> Body { get; } = new Dictionary<string, object>();
	}
}