namespace Convenient.Dump.Core.App.Queries.Lang
{
	public struct QueryToken
	{
		public TokenType Type { get; }
		public SourcePosition Position { get; }
		public string Value { get; }

		public QueryToken(TokenType type, SourcePosition position, string value)
		{
			Type = type;
			Position = position;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Position}: {Type} {Value}";
		}
	}
}