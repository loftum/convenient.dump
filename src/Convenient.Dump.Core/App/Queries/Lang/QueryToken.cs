namespace Convenient.Dump.Core.App.Queries.Lang
{
	public struct QueryToken
	{
		public TokenType Type { get; }
		public SourcePosition Position { get; }
		public string RawValue { get; }
		public object Value { get; }

		public QueryToken(TokenType type, SourcePosition position, string rawValue) : this(type, position, rawValue, rawValue)
		{
		}

		public QueryToken(TokenType type, SourcePosition position, string rawValue, object value)
		{
			Type = type;
			Position = position;
			RawValue = rawValue;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Position}: {Type} {Value}";
		}
	}
}