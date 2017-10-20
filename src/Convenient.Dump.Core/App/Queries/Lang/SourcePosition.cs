namespace Convenient.Dump.Core.App.Queries.Lang
{
	public struct SourcePosition
	{
		public int LineNumber { get; }
		public int ColumnNumber { get; }

		public SourcePosition(int lineNumber, int columnNumber)
		{
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
		}

		public override string ToString()
		{
			return $"({LineNumber}:{ColumnNumber})";
		}
	}
}