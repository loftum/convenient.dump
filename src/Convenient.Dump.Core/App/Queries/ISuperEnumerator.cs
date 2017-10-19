using System.Collections.Generic;

namespace Convenient.Dump.Core.App.Queries
{
	public interface ISuperEnumerator<out T> : IEnumerator<T>
	{
		bool Moved { get; }
	}
}