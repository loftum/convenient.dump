namespace Convenient.Dump.Core.App.Queries.Lang
{
	public struct QueryToken
	{
		public TokenType Type { get; }
		public SourcePosition Position { get; }
		public string StringValue { get; }
		public object Value { get; }

		public QueryToken(TokenType type, SourcePosition position, string stringValue) : this(type, position, stringValue, stringValue)
		{
		}

		public QueryToken(TokenType type, SourcePosition position, string stringValue, object value)
		{
			Type = type;
			Position = position;
			StringValue = stringValue;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Position}: {Type} {Value}";
		}
	}
}