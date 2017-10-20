using System;

namespace Convenient.Dump.Core.App.Queries.Lang
{
	public class QueryParserException : Exception
	{
		public QueryParserException(SourcePosition position, string message) : base((string) Format(position, message))
		{
		}

		private static string Format(SourcePosition position, string message)
		{
			return $"{position}: {message}";
		}

		public static QueryParserException UnexpectedEndAt(QueryToken token)
		{
			return new QueryParserException(token.Position, "Unexpected end of query");
		}
	}
}