using Convenient.Dump.Core.App.Queries.Nodes;

namespace Convenient.Dump.Core.Data
{
	public class QueryInput
	{
		public int Skip { get; set; }
		public int Take { get; set; } = 20;
		public QueryNode Query { get; set; }
	}
}