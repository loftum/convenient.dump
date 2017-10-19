namespace Convenient.Dump.Core.App.Queries.Nodes
{
	public class ConstantNode : QueryNode
	{
		public string Value { get; }

		public ConstantNode(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}