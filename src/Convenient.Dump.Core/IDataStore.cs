using System.Threading.Tasks;
using Convenient.Dump.Core.Data;

namespace Convenient.Dump.Core
{
	public interface IDataStore
	{
		Task Save(string collection, string json);
		Task<DbInfo> GetInfo();
		Task<QueryResult> Query(string collection, QueryParameters parameters);
		Task DropCollection(string collection);
		Task<bool> RemoveItem(string collection, string id);
	}
}