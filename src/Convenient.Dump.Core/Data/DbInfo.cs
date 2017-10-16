using System.Collections.Generic;
using System.Linq;

namespace Convenient.Dump.Core.Data
{
	public class DbInfo
	{
		public CollectionInfo[] Collections { get; }

		public DbInfo(IEnumerable<CollectionInfo> collections)
		{
			Collections = collections.ToArray();
		}
	}

	public class CollectionInfo
	{
		public string Name { get; }
		public int Count { get; }

		public CollectionInfo(string name, int count)
		{
			Name = name;
			Count = count;
		}
	}
}