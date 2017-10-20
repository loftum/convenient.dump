using System.Collections.Generic;

namespace Convenient.Dump.Core.App.Queries.Lang
{
	public interface ISuperEnumerator<out T> : IEnumerator<T>
	{
		bool Moved { get; }
	}
}