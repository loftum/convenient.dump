using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Convenient.Dump.Core;
using Convenient.Dump.Core.Data;
using LiteDB;

namespace ConvenientDump.LiteDb
{
	public class LiteDbDataStore : IDataStore
	{
		private readonly LiteDatabase _db;

		public LiteDbDataStore()
		{
			_db = new LiteDatabase("Dump.db");
		}

		public LiteDbDataStore(LiteDatabase db)
		{
			_db = db;
		}

		public Task Save(string collection, string json)
		{
			var document = JsonSerializer.Deserialize(json) as BsonDocument;
			if (document == null)
			{
				return Task.CompletedTask;
			}
			var coll = _db.GetCollection(collection);
			coll.Upsert(document);
			return Task.CompletedTask;
		}

		public Task<object> Get(string collection, string id)
		{
			var coll = _db.GetCollection(collection);
			var result = coll.FindById(new ObjectId(id));
			return Task.FromResult((object)result);
		}

		public Task<bool> RemoveItem(string collection, string id)
		{
			var coll = _db.GetCollection(collection);
			var result = coll.Delete(new ObjectId(id));
			return Task.FromResult(result);
		}

		public Task<QueryResult> QueryCollection(string collection, QueryInput input)
		{
			var coll = _db.GetCollection(collection);
			var query = input.Query == null ? Query.All() : new QueryVisitor().Visit(input.Query);

			var result = new QueryResult
			{
				Items = coll.Find(query, input.Skip, input.Take).Select(ToObject).ToArray()
			};
			return Task.FromResult(result);
		}

		public Task DropCollection(string collection)
		{
			_db.DropCollection(collection);
			return Task.CompletedTask;
		}

		public Task<DbInfo> GetInfo()
		{
			var collections = _db.GetCollectionNames().Select(n => new CollectionInfo(n, _db.GetCollection(n).Count()));
			return Task.FromResult(new DbInfo(collections));
		}

		private Dictionary<string, object> ToDictionary(BsonDocument doc)
		{
			if (doc == null)
			{
				return null;
			}
			var result = new Dictionary<string, object>();
			foreach (var key in doc.Keys)
			{
				result[key] = ToObject(doc[key]);
			}
			return result;
		}

		private object ToObject(BsonValue value)
		{
			switch (value)
			{
				case BsonDocument doc: return ToDictionary(doc);
				case BsonArray array: return array.Select(ToObject);
				case var v when v.IsObjectId: return v.AsObjectId.ToString();
				default: return value.RawValue;
			}
		}
	}
}