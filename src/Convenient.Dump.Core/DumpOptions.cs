using System;

namespace Convenient.Dump.Core
{
	public class DumpOptions
	{
		public IDataStore DataStore { get; set; }
		public Func<object, string> ToJson { get; set; }
	}
}