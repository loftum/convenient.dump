# Convenient dump

This is a ASP.NET Core framework for dumping json blobs over http. It can be used for storing log entries, events or debug data.

## Api
- POST /{collection} -> Stores request body (json) in a collection
- GET / -> Prints collection info (names and document count)
- GET /{collection} -> Query a collection
- DELETE /{collection} -> Drops the collection
- GET /{collection}/{id} -> Get a specific document
- DELETE /{collection}/{id} -> Deletes a specific document
