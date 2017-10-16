using System.Collections.Generic;

namespace Convenient.Dump.Core.Data
{
	public class QueryParameters
	{
		public int Skip { get; set; }
		public int Take { get; set; } = 20;
		public Dictionary<string, object> Fields { get; } = new Dictionary<string, object>();
	}
}