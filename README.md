# Convenient dump

This is a ASP.NET Core framework for dumping json blobs over http. It can be used for storing log entries, events or debug data.

## Api
- GET / -> Get db info (collection names and document count)
- POST /{collection} -> Stores request body (json) in a collection
- GET /{collection} -> Query a collection
- DELETE /{collection} -> Drops the collection
- DELETE /{collection}/* -> Clears collection (deletes all documents)
- GET /{collection}/{id} -> Get a specific document
- DELETE /{collection}/{id} -> Deletes a specific document
